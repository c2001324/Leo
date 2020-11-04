using HexMap;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 角色的动画控件器
/// </summary>
public class CAttackComponent : CComponent, IDamageCaster
{


    CPropertyComponent m_PropertyComponent;

    CCreateDamageComponent m_CreateDamageComponent;

    CAnimatorComponent m_AnimatorComponent;

    public bool isAttacking { get; private set; }

    protected override void OnInitializeComplete()
    {
        base.OnInitializeComplete();
        m_PropertyComponent = entity.GetCComponent<CPropertyComponent>();
        m_AnimatorComponent = entity.GetCComponent<CAnimatorComponent>();
        m_CreateDamageComponent = entity.GetCComponent<CCreateDamageComponent>();
        isAttacking = false;
    }

    public List<HexCell> GetAttackRange()
    {
        HexRoom.HexCellPaths path = LevelManager.room.hexRoom.GetAvailableDestinations(entity.centerCell, (int)m_PropertyComponent.attackRange + (int)m_PropertyComponent.movePoint);
        return path.GetDestinations();
    }

    public bool Attack(Entity target)
    {
        if (isAttacking)
        {
            return false;
        }
        else
        {
            isAttacking = true;
            m_AnimatorComponent.SpellAbility();
            //播放动画
            DamageData damageData = new DamageData(this)
            {
                damage = 1
            };
            m_CreateDamageComponent.ApplyDamage(damageData, target);
            TimerManager.instance.AddDelayTimer(OnAttackComplete, null, 0.5f);
            return true;
        }
    }

    void OnAttackComplete(uint oid, object param)
    {
        if (isAttacking)
        {
            isAttacking = false;
            onAttackComplete.Invoke();
        }
    }

    public Entity GetEntity()
    {
        return entity;
    }

    public AttackEvent onAttackComplete = new AttackEvent();

    public class AttackEvent : CustomEvent { }
}
