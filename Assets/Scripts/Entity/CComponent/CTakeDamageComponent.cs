using System.Collections.Generic;


public class CTakeDamageComponent : CComponent
{

    public CPropertyComponent propertyComponent { get; private set; }

    public CTeamComponent relationComponent { get; private set; }

    public ModifyDamageMark modifyDamageMark { get; private set; }

    public DamageResist damageResist { get; private set; }

    public DamageShield damageShield { get; private set; }

    public DamageGain damageGain { get; private set; }

    public DodgeDamage dodgeDamage { get; private set; }

    public DamageBlock damageBlock { get; private set; }

    CAnimatorComponent m_AnimatorComponent;

    protected override void OnInitializeComplete()
    {
        base.OnInitializeComplete();
        propertyComponent = entity.GetCComponent<CPropertyComponent>();
        relationComponent = entity.GetCComponent<CTeamComponent>();
        m_AnimatorComponent = entity.GetCComponent<CAnimatorComponent>();
        damageResist = new DamageResist();
        damageShield = new DamageShield();
        damageGain = new DamageGain();
        modifyDamageMark = new ModifyDamageMark();
        dodgeDamage = new DodgeDamage();
        damageBlock = new DamageBlock();
    }

    public bool PreTakeDamage(Damage damage)
    {
        //修改伤害标记
        modifyDamageMark.Process(damage);
        if (entity.CheckFlags(EntityFlags.Invulnerable))
        {
            return false;
        }
        else if (entity.CheckFlags(EntityFlags.PhysicImmune) && damage.damageType == DamageType.Physic)
        {
            return false;
        }
        else if (entity.CheckFlags(EntityFlags.MagicImmune) && damage.damageType == DamageType.Magic)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool TakeDamage(Damage damage)
    {
        if (active)
        {
            //处理伤害闪避
            if (dodgeDamage.CheckIsDodgeDamage(damage))
            {
                onDodgeDamage.Invoke(damage);
            }
            else
            {
                //接收伤害
                ProcessReceiveDamage(damage);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 计算出伤害后，开始接收伤害
    /// </summary>
    /// <param name="damage"></param>
    void ProcessReceiveDamage(Damage damage)
    {
        //伤害加深
        damageGain.Process(damage);
        
        float blockDamage = 0f;
        object blockKey = null;
        DamageBlock.BlockResult blockResult = damageBlock.IsBlockDamage(ref damage, out blockKey, out blockDamage);
        if (blockResult == DamageBlock.BlockResult.BlockSuccess)
        {
            //完美格档
            onBlockDamageSuccess.Invoke(damage, blockKey, blockDamage);
        } 
        else if (blockResult == DamageBlock.BlockResult.BlockFailed)
        {
            onBlockDamageFailed.Invoke(damage, blockKey);
        }

        if (blockResult != DamageBlock.BlockResult.BlockSuccess)
        {
            //计算伤害的抗性
            damageResist.Process(ref damage);
            float shieldValue = 0f;
            //计算护盾
            damageShield.Process(ref damage, out shieldValue);
            if (shieldValue > 0f)
            {
                onShieldDamage.Invoke(damage, shieldValue);
            }
        }
        
        //处理一些特殊的标记
        damage.ProcessForceModifyDamageMark();
        //锁定伤害
        damage.LockDamage();
        //在以下的流程里，不会再对Damage进行修改了
        if (damage.receiveDamageValue > 0)
        {
            //扣血
            propertyComponent.TakeDamage(damage);
            //在接收伤害后触发暴击的消息
            if (damage.CheckActionMark(DamageResultMark.Crit))
            {
                damage.creator.OnCritToOther(damage);
                OnReceiverCrit(damage);
            }
            onTakeDamage.Invoke(damage);
            damage.OnTakeDamage();
        }
        //播放动画
        m_AnimatorComponent.GetHit();
    }

    /// <summary>
    /// 受到攻击前的回调，不一定会被攻击中，可能miss
    /// </summary>
    /// <param name="target"></param>
    public void OnBeforeUnderAttack(CCreateDamageComponent target)
    {
        onBeforeUnderAttack.Invoke(target);
    }

    /// <summary>
    /// 被暴击
    /// </summary>
    /// <param name="damage"></param>
    void OnReceiverCrit(Damage damage)
    {
        onReceiveCrit.Invoke(damage);
    }


    public BeforeUnderAttackEvent onBeforeUnderAttack = new BeforeUnderAttackEvent();

    /// <summary>
    /// 只给SpellSkill来使用，以保证SpellSkill是第一个收到这个消息的
    /// </summary>
    public TakeDamageEvent onProcess = new TakeDamageEvent();

    public TakeDamageEvent onTakeDamage = new TakeDamageEvent();
    public ReceiveCritEvent onReceiveCrit = new ReceiveCritEvent();
    public DodgeDamageEvent onDodgeDamage = new DodgeDamageEvent();

    public ShieldDamageEvent onShieldDamage = new ShieldDamageEvent();

    public PrefectBlockDamageSuccessEvent onBlockDamageSuccess = new PrefectBlockDamageSuccessEvent();
    public PrefectBlockDamageFailedEvent onBlockDamageFailed = new PrefectBlockDamageFailedEvent();

    public class DodgeEvent : CustomEvent<CCreateDamageComponent> { }
    public class BeforeUnderAttackEvent : CustomEvent<CCreateDamageComponent> { }
    public class TakeDamageEvent : CustomEvent<Damage> { }
    public class BeforeReceiveDamageEvent : CustomEvent<Damage> { }
    public class ReceiveCritEvent : CustomEvent<Damage> { }
    public class DodgeDamageEvent : CustomEvent<Damage> { }
    public class ShieldDamageEvent : CustomEvent<Damage, float> { }
    public class PrefectBlockDamageSuccessEvent : CustomEvent<Damage, object, float> { }
    public class PrefectBlockDamageFailedEvent : CustomEvent<Damage, object> { }
}
