using UnityEngine;

public static class SkillActionIndex
{
    public static readonly int channelSpellIndex = -1;  //持续施法的默认动作id
    public static readonly int chargeSpellIndex = -2;   //充能施法的默认动作id
    public static readonly int defaultSkill = 0;
    public static readonly int playerDash = 9;
    public static readonly int weaponAttack = 10;       //武器攻击从10开妈
}

public class AnimatorParamName
{
    public static readonly string moving = "Moving";
    public static readonly string spellIndex = "SpellIndex";
    public static readonly string triggerSpellAbility = "TriggerSpellAbility";    //开始施法时
    public static readonly string triggerGetHit = "TriggerGetHit";

    public static readonly string triggerDie = "TriggerDie";
    public static readonly string dying = "Dying";
    public static readonly string triggerReborn = "TriggerReborn";
    public static readonly string triggerStunned = "TriggerStunned";
}

/// <summary>
/// 角色的动画控件器
/// </summary>
public class CAnimatorComponent : CComponent
{
    Animator m_Animator;

    
    protected virtual void ResetStateTrigger()
    {
        m_Animator.ResetTrigger(AnimatorParamName.triggerGetHit);

        m_Animator.ResetTrigger(AnimatorParamName.triggerReborn);
        m_Animator.ResetTrigger(AnimatorParamName.triggerDie);
        m_Animator.ResetTrigger(AnimatorParamName.triggerStunned);
    }


    void ResetSpellTrigger()
    {
        m_Animator.ResetTrigger(AnimatorParamName.triggerSpellAbility);
    }

    protected override void OnInitializeComplete()
    {
        base.OnInitializeComplete();
        OnSetModel(entity.entityModel);
        entity.modelComponent.onSetModel.AddListener(OnSetModel);
        entity.modelComponent.onRemoveModel.AddListener(OnRemoveModel);
    }


    protected override void OnDestroyComplete()
    {
        base.OnDestroyComplete();
        entity.modelComponent.onSetModel.RemoveListener(OnSetModel);
        entity.modelComponent.onRemoveModel.RemoveListener(OnRemoveModel);
    }

    void OnSetModel(EntityModel model)
    {
        m_Animator = model.GetAnimator();
    }

    void OnRemoveModel(EntityModel model)
    {
        m_Animator = null;
    }

    public void SpellAbility()
    {
        m_Animator.SetTrigger(AnimatorParamName.triggerSpellAbility);
    }

    public void Reborn()
    {
        m_Animator.SetBool(AnimatorParamName.triggerReborn, true);
    }

    public void Idle()
    {
        m_Animator.SetBool(AnimatorParamName.moving, false);
    }

    public void Move()
    {
        m_Animator.SetBool(AnimatorParamName.moving, true);
    }

    public void GetHit()
    {
        ResetStateTrigger();
        m_Animator.SetTrigger(AnimatorParamName.triggerGetHit);
    }


    public virtual void Die()
    {
        ResetStateTrigger();
        m_Animator.SetTrigger(AnimatorParamName.triggerDie);
    }

}
