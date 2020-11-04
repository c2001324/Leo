using UnityEngine;

public interface IDamageCaster
{
    Entity GetEntity();
}

public class Damage
{
    public Damage(uint batchOid, 
        IDamageCaster caster, 
        CCreateDamageComponent creator, 
        CTakeDamageComponent receiver, 
        DamageSource source, 
        DamageType type, 
        float damageValue, 
        float getHitTime, 
        float hitBack, 
        float damageInterval,
        DamageMark mark)
    {
        time = Time.time;
        if (caster == null)
        {
            Debug.LogError("caster 不可以为空");
        }
        this.batchOid = batchOid;
        this.caster = caster;
        this.creator = creator;
        this.receiver = receiver;
        m_SourceDamageValue = damageValue;
        receiveDamageValue = damageValue;
        damageType = type;
        damageSource = source;
        this.hitBack = hitBack;
        this.getHitTime = getHitTime;
        this.damageInterval = damageInterval;

        if (this.getHitTime <= 0)
        {
            this.getHitTime = Config.combat.getHitTime;
        }
        if (this.damageInterval <= 0)
        {
            this.damageInterval = Config.combat.damageInterval;
        }

        damageMark = mark;
        m_DamageActionMark = DamageResultMark.None;
        m_Lock = false;
    }

    /// <summary>
    /// 同一个DamageData产生的Damage是同一个批次id
    /// </summary>
    public uint batchOid { get; private set; }

    /// <summary>
    /// 锁住，是否可以修改Damage的内容
    /// </summary>
    bool m_Lock = false;

    /// <summary>
    /// 创建伤害的技能，不可以为空
    /// </summary>
    public IDamageCaster caster { get; private set; }

    /// <summary>
    /// 伤害创建者
    /// </summary>
    public CCreateDamageComponent creator { get; private set; }

    /// <summary>
    /// 伤害接收者
    /// </summary>
    public CTakeDamageComponent receiver { get; private set; }

    /// <summary>
    /// 原始的伤害值
    /// </summary>
    float m_SourceDamageValue;

    /// <summary>
    /// 伤害类型
    /// </summary>
    public DamageType damageType { get; private set; }

    /// <summary>
    /// 伤害来源
    /// </summary>
    public DamageSource damageSource { get; private set; }

    /// <summary>
    /// 受击的硬直时间
    /// </summary>
    public float getHitTime { get; private set; }

    /// <summary>
    /// 最后真正接收到的伤害值
    /// </summary>
    public float receiveDamageValue { get; private set; }

    /// <summary>
    /// 受伤被击退
    /// </summary>
    public float hitBack { get; private set; }

    /// <summary>
    /// 伤害的最小间隔
    /// </summary>
    public float damageInterval { get; private set; }

    //伤害标记
    public DamageMark damageMark { get; private set; }

    //伤害的创建时间
    public float time { get; private set; }

    /// <summary>
    /// 是否播放伤害动画
    /// </summary>
    public bool playDamageAnimation
    {
        get
        {
            if (CheckMark(DamageMark.ForbidDamageAnimation))
            {
                return false;
            }
            else
            {
                if (CheckActionMark(DamageResultMark.BlockDamageSuccess))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }

    /// <summary>
    /// 由本次伤害造成死亡
    /// </summary>
    public void OnDead()
    {
        if (creator != null)
        {
            creator.OnKillOther(this);
        }
    }

    /// <summary>
    /// 处理特殊的标记
    /// </summary>
    public void ProcessForceModifyDamageMark()
    {
        if (!m_Lock)
        {
            if (CheckMark(DamageMark.ForceUndead))
            {
                //与ForceKill同时存在时，无视ForceKill
                if (receiveDamageValue >= receiver.propertyComponent.hp)
                {
                    //保留一点血
                    ModifyDamageByForced(receiver.propertyComponent.hp - 1f);
                }
            }
            else if (CheckMark(DamageMark.ForceKill))
            {
                ModifyDamageByForced(receiver.propertyComponent.hpMax);
            }
        }
        else
        {
            Debug.LogError("非法修改Damage！");
        }
    }

    /// <summary>
    /// 锁定，不可以再修改Damage
    /// </summary>
    public void LockDamage()
    {
        //向上取整数
        SetDamage(Untility.Tool.HpFloat2Int(receiveDamageValue), true);
        m_Lock = true;
    }

    /// <summary>
    /// 目标接收伤害后的回调
    /// </summary>
    public void OnTakeDamage()
    {
        creator.OnTargetTakeDamage(this);
    }

    #region 调整伤害值
    /// <summary>
    /// 通过护盾修改伤害
    /// </summary>
    /// <param name="damage"></param>
    public bool ModifyDamageByShield(float damage)
    {
        if (m_Lock)
        {
            Debug.LogError("非法修改Damage！");
        }
        if (!m_Lock && !CheckMark(DamageMark.ForbidModifyDamageByShield))
        {
            AddResultMark(DamageResultMark.ShieldDamage);
            SetDamage(damage, true);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 通过抗性和攻防计算修改伤害
    /// </summary>
    /// <param name="damage"></param>
    public bool ModifyDamageByResist(float damage)
    {
        if (m_Lock)
        {
            Debug.LogError("非法修改Damage！");
        }
        if (!m_Lock && !CheckMark(DamageMark.ForbidModifyDamageByResist))
        {
            AddResultMark(DamageResultMark.ResistDamage);
            SetDamage(damage, false);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 伤害加深修改伤害
    /// </summary>
    /// <param name="damage"></param>
    public bool ModifyDamageByDamageGain(float damage)
    {
        if (m_Lock)
        {
            Debug.LogError("非法修改Damage！");
        }
        if (!m_Lock && !CheckMark(DamageMark.ForbidModifyDamageByGain))
        {
            AddResultMark(DamageResultMark.GainDamage);
            SetDamage(damage, false);
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// 通过暴击修改伤害
    /// </summary>
    /// <param name="damage"></param>
    public void ModifyDamageByCrit(float damage)
    {
        if (m_Lock)
        {
            Debug.LogError("非法修改Damage！");
        }
        if (!m_Lock && !CheckMark(DamageMark.ForbidCrit))
        {
            AddResultMark(DamageResultMark.Crit);
            SetDamage(damage, false);
        }
    }

    /// <summary>
    /// 通过强制修改伤害，不受任何标记影响
    /// </summary>
    /// <param name="damage"></param>
    public void ModifyDamageByForced(float damage)
    {
        if (m_Lock)
        {
            Debug.LogError("非法修改Damage！");
        }
        if (!m_Lock)
        {
            SetDamage(damage, true);
        }
    }

    /// <summary>
    /// 完美格档
    /// </summary>
    public void ModifyDamageByPerfectBlock(bool blockSuccess)
    {
        if (!m_Lock)
        {
            if (blockSuccess)
            {
                AddResultMark(DamageResultMark.BlockDamageSuccess);
                SetDamage(0f, true);
            }
            else
            {
                AddResultMark(DamageResultMark.BlockDamageFailed);
            }
        }
    }

    void SetDamage(float damage, bool forced)
    {
        if (m_Lock)
        {
            Debug.LogError("非法修改Damage！");
        }

        if (!m_Lock)
        {
            receiveDamageValue = damage;
            if (!forced && !CheckMark(DamageMark.ForbidMinDamageLimit) && receiveDamageValue < 1)
            {
                //最少造成1点伤害
                ModifyDamageByForced(1f);
            }
            else
            {
                receiveDamageValue = receiveDamageValue > 0 ? receiveDamageValue : 0;
            }
        }
    }
    #endregion

    #region 伤害标记

    /// <summary>
    /// 检查标记是否存在
    /// </summary>
    /// <param name="mark"></param>
    /// <returns></returns>
    public bool CheckMark(DamageMark mark)
    {
        if (damageMark == DamageMark.None || mark == DamageMark.None)
        {
            return false;
        }
        return (damageMark & mark) == mark;
    }

    /// <summary>
    /// 添加禁止为行的标记
    /// </summary>
    /// <param name="mark"></param>
    public void AddMark(DamageMark mark)
    {
        if (m_Lock)
        {
            Debug.LogError("非法修改Damage！");
        }
        if (!m_Lock && mark != DamageMark.None && !CheckMark(mark))
        {
            damageMark |= mark;
        }
    }

    /// <summary>
    /// 删除禁止行为的标记
    /// </summary>
    /// <param name="mark"></param>
    public void RemoveMark(DamageMark mark)
    {
        if (m_Lock)
        {
            Debug.LogError("非法修改Damage！");
        }
        if (!m_Lock && mark != DamageMark.None)
        {
            damageMark &= ~mark;
        }
    }

    /// <summary>
    /// 检查 sourceMark 是否包括 targetMark
    /// </summary>
    /// <param name="sourceMark"></param>
    /// <param name="targetMark"></param>
    /// <returns></returns>
    public static bool CheckMark(DamageMark sourceMark, DamageMark targetMark)
    {
        return (sourceMark & targetMark) == targetMark;
    }

    #endregion

    /// <summary>
    /// 是否为同一个伤害类型
    /// </summary>
    public static bool IsSameDamageType(DamageType type, DamageType otherType)
    {
        if (type == DamageType.All || otherType == DamageType.All)
        {
            return true;
        }
        else if (type != DamageType.Magic && otherType != DamageType.Magic)
        {
            return type == otherType;
        }
        else if (type == DamageType.Magic)
        {
            return IsMagicDamageType(otherType);
        }
        else
        {
            return IsMagicDamageType(type);
        }
    }

    /// <summary>
    /// 是否为魔法伤害
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsMagicDamageType(DamageType type)
    {
        return type == DamageType.Magic;
    }

    /// <summary>
    /// 是否为同一个伤害来源
    /// </summary>
    public static bool IsSameDamageSource(DamageSource source, DamageSource otherSource)
    {
        if (source == DamageSource.All || otherSource == DamageSource.All)
        {
            return true;
        }
        else
        {
            return source == otherSource;
        }
    }

    #region 为行标记
    DamageResultMark m_DamageActionMark;
    /// <summary>
    /// 检查标记是否存在
    /// </summary>
    /// <param name="mark"></param>
    /// <returns></returns>
    public bool CheckActionMark(DamageResultMark mark)
    {
        if (m_DamageActionMark == DamageResultMark.None || mark == DamageResultMark.None)
        {
            return false;
        }
        return (m_DamageActionMark & mark) == mark;
    }

    void AddResultMark(DamageResultMark mark)
    {
        if (m_Lock)
        {
            Debug.LogError("非法修改Damage！");
        }
        if (!m_Lock && mark != DamageResultMark.None && !CheckActionMark(mark))
        {
            m_DamageActionMark |= mark;
        }
    }


    #endregion
}

public enum DamageType
{
    Physic,
    Magic,
    Pure,
    All
}

public enum DamageSource
{
    Attack,
    Bomb,
    Skill,
    Buff,
    All
}

public enum DamageValueType
{
    Point,
    MaxHpPercent,
    CurHpPercent,
    Custom
}