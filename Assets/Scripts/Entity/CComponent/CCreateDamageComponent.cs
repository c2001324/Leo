using System.Collections.Generic;


public class CCreateDamageComponent : CComponent
{
    public CTeamComponent teamComponent { get; private set; }

    public ModifyDamageMark modifyDamageMark { get; private set; }

    public AttackCrit attackCrit { get; private set; }

    public DamageGain damageGain { get; private set; }

    protected override void OnInitializeComplete()
    {
        base.OnInitializeComplete();
        teamComponent = entity.GetCComponent<CTeamComponent>();
        modifyDamageMark = new ModifyDamageMark();
        attackCrit = new AttackCrit();
        damageGain = new DamageGain();
    }

    protected override void OnDestroyComplete()
    {
        base.OnDestroyComplete();
        modifyDamageMark.Destroy();
        attackCrit.Destroy();
    }
    

    public void ApplyDamage(DamageData damageData, Entity[] targets)
    {
        foreach (Entity target in targets)
        {
            ApplyDamage(damageData, target);
        }
    }

    public void ApplyDamage(DamageData damageData, Entity target)
    {
        ApplyDamage(damageData, target.GetCComponent<CTakeDamageComponent>());
    }

    public void ApplyDamage(DamageData damageData, CTakeDamageComponent[] targets)
    {
        if (active)
        {
            foreach (CTakeDamageComponent target in targets)
            {
                ApplyDamage(damageData, target);
            }
        }
    }

    public bool ApplyDamage(DamageData damageData, CTakeDamageComponent target)
    {
        if (!active || target == null)
        {
            return false;
        }
        //生成伤害数据
        Damage damage = CreateDamage(damageData, target);
        if (damage != null)
        {
            //修改伤害标记
            modifyDamageMark.Process(damage);
            //接收伤害的预处理
            if (!target.PreTakeDamage(damage))
            {
                return false;
            }
            //检查敌对关系
            if (!Damage.CheckMark(damageData.mark, DamageMark.ForbidCheckRelation) && !CheckRelation(target))
            {
                return false;
            }
            //攻击前检查
            if (damageData.damageSource == DamageSource.Attack)
            {
                PreProcessAttack(target);
            }
            //处理伤害数据
            ProcessDamage(damage);
            //发送伤害数据
            target.TakeDamage(damage);
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// 生成Damage数据
    /// </summary>
    Damage CreateDamage(DamageData damageData, CTakeDamageComponent target)
    {
        if (active && target != null && target.active)
        {
            //创建伤害
            float damageValue = damageData.GetDamageValue(target);
            Damage damage = new Damage(damageData.batchOid, damageData.caster, this, target, damageData.damageSource, 
                damageData.damageType, damageValue, damageData.getHitTime, damageData.hitBack, damageData.damageInterval, damageData.mark);
            return damage;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 处理伤害数据
    /// </summary>
    /// <param name="damage"></param>
    void ProcessDamage(Damage damage)
    {
        //处理伤害增益
        damageGain.Process(damage);
        //处理暴击
        if (!damage.CheckMark(DamageMark.ForbidCrit) && damage.damageSource == DamageSource.Attack)
        {
            float critValue = attackCrit.GetCritValue(damage);
            if (critValue > 1f)
            {
                damage.ModifyDamageByCrit(damage.receiveDamageValue * critValue);
            }
        }
    }


    /// <summary>
    /// 击杀目标后的回调
    /// </summary>
    /// <param name="receiver"></param>
    public void OnKillOther(Damage damage)
    {
        onKillOther.Invoke(damage);
    }

    /// <summary>
    /// 攻击目标触发了暴击
    /// </summary>
    /// <param name="damage"></param>
    public void OnCritToOther(Damage damage)
    {
        onCritToOther.Invoke(damage);
    }

    public void OnTargetTakeDamage(Damage damage)
    {
        if (damage.damageSource == DamageSource.Attack)
        {
            onAttack.Invoke(damage);
        }
        onTargetTakeDamage.Invoke(damage);
    }

    /// <summary>
    /// 预处理攻击游戏
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    void PreProcessAttack(CTakeDamageComponent target)
    {
        onBeforeAttack.Invoke(target);
        target.OnBeforeUnderAttack(this);
    }

    bool CheckRelation(CTakeDamageComponent target)
    {
        return CTeamComponent.GetRelation(teamComponent, target.relationComponent) == Relation.Enemy;
    }

    readonly public BeforeAttackEvent onBeforeAttack = new BeforeAttackEvent();
    readonly public DamageEvent onAttack = new DamageEvent();
    readonly public AttackMissEvent onAttackMiss = new AttackMissEvent();
    readonly public DamageEvent onKillOther = new DamageEvent();
    readonly public DamageEvent onTargetTakeDamage = new DamageEvent();
    readonly public DamageEvent onCritToOther = new DamageEvent();

    public class AttackMissEvent : CustomEvent<CTakeDamageComponent> { }

    public class BeforeAttackEvent : CustomEvent<CTakeDamageComponent> { }

    public class DamageEvent : CustomEvent<Damage> { }
}

/// <summary>
/// 创建伤害的数据
/// </summary>
public class DamageData
{
    public DamageData(IDamageCaster caster)
    {
        this.caster = caster;
        batchOid = Untility.GuidCreator.GetOid();
        damageSource = DamageSource.Attack;
        damageType = DamageType.Physic;
        valueType = DamageValueType.Point;
        damage = 0f;
        getCustomDamageHandle = null;
        getHitTime = Config.combat.getHitTime;
        hitBack = 0f;
        damageInterval = Config.combat.damageInterval;
        mark = DamageMark.None;
    }

    public uint batchOid { get; private set; }

    public IDamageCaster caster { get; private set; }

    public DamageSource damageSource { get; set; }

    public DamageType damageType { get; set; }

    public DamageValueType valueType { get; set; }

    public float damage { get; set; }

    public GetCustomDamageDelegate getCustomDamageHandle { get; set; }

    public float hitBack { get; set; }

    public float getHitTime { get; set; }

    public float damageInterval { get; set; }

    public DamageMark mark { get; set; }

    public float GetDamageValue(CTakeDamageComponent target)
    {
        switch (valueType)
        {
            case DamageValueType.Point:
                return damage;
            case DamageValueType.MaxHpPercent:
                return target.propertyComponent.hpMax * damage;
            case DamageValueType.CurHpPercent:
                return target.propertyComponent.hp * damage;
            case DamageValueType.Custom:
                return getCustomDamageHandle(target);
            default:
                return 0;
        }
    }

    public delegate float GetCustomDamageDelegate(CTakeDamageComponent target);
}
