using System.Collections.Generic;

/// <summary>
/// 护盾
/// 每个护盾单独计算
/// </summary>
public class DamageShield
{

    public DamageShield()
    {

    }

    public ShieldData AddShield(object effect, ValueType valueType, float value, int blockCount, DamageType damageType, DamageSource damageSource, CheckDamageDelegate handle)
    {
        ShieldData data = new ShieldData(valueType, value, blockCount, damageType, damageSource, handle);
        if (valueType == ValueType.Point)
        {
            if (m_PointValue.ContainsKey(effect))
            {
                m_PointValue[effect] = data;
            }
            else
            {
                m_PointValue.Add(effect, data);
            }
        }
        else
        {
            if (m_PercentValue.ContainsKey(effect))
            {
                m_PercentValue[effect] = data;
            }
            else
            {
                m_PercentValue.Add(effect, data);
            }
        }
        return data;
    }

    public void RemoveShield(object effect)
    {
        m_PointValue.Remove(effect);
        m_PercentValue.Remove(effect);
    }

    /// <summary>
    /// 处理
    /// </summary>
    /// <param name="damageType"></param>
    /// <param name="damageSource"></param>
    public void Process(ref Damage damage, out float blockValue)
    {
        if (damage.CheckMark(DamageMark.ForbidModifyDamageByShield))
        {
            blockValue = 0f;
            return;
        }
        float sourceDamage = damage.receiveDamageValue;
        //计算点数
        Process(damage, m_PointValue);
        //计算百分比
        Process(damage, m_PercentValue);
        blockValue = sourceDamage - damage.receiveDamageValue;
    }

    void Process(Damage damage, Dictionary<object, ShieldData> datas)
    {
        foreach (object key in datas.Keys)
        {
            if (damage.receiveDamageValue <= 0)
            {
                break;
            }
            ShieldData data = datas[key];
            if (data.Process(damage))
            {
                onTriggerShield.Invoke(key, data);
                if (!data.valid)
                {
                    m_RemoveCacha.Add(key);
                }
            }
        }

        foreach (object key in m_RemoveCacha)
        {
            datas.Remove(key);
        }
        m_RemoveCacha.Clear();
    }

    List<object> m_RemoveCacha = new List<object>();

    Dictionary<object, ShieldData> m_PointValue = new Dictionary<object, ShieldData>();
    Dictionary<object, ShieldData> m_PercentValue = new Dictionary<object, ShieldData>();

    public class ShieldData
    {
        public ShieldData(ValueType valueType, float value, int blockCount, DamageType type, DamageSource source, CheckDamageDelegate handle)
        {
            this.valueType = valueType;
            this.value = value;
            this.blockCount = blockCount;
            m_Handle = handle;
            remaingingCount = blockCount;
            damageType = type;
            damageSource = source;
        }

        readonly public ValueType valueType;
        readonly public float value;
        readonly public int blockCount;
        public int remaingingCount { get; private set; }

        public bool valid
        {
            get
            {
                if (blockCount > 0)
                {
                    return remaingingCount > 0;
                }
                else
                {
                    return true;
                }
            }
        }
        
        readonly public DamageType damageType;
        readonly public DamageSource damageSource;
        
        public bool Process(Damage damage)
        {
            if (valid && damage.receiveDamageValue > 0 && CheckDamage(damage))
            {
                if (valueType == ValueType.Point)
                {
                    damage.ModifyDamageByShield(damage.receiveDamageValue - value);
                }
                else
                {
                    damage.ModifyDamageByShield(damage.receiveDamageValue - (damage.receiveDamageValue * value));
                }
                if (remaingingCount > 0)
                {
                    remaingingCount--;
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        bool CheckDamage(Damage damage)
        {
            if (Damage.IsSameDamageSource(damageSource, damage.damageSource) &&
                Damage.IsSameDamageType(damageType, damage.damageType))
            {
                return m_Handle == null || m_Handle(damage);
            }
            else
            {
                return false;
            }
        }

        CheckDamageDelegate m_Handle;
    }

    public readonly TriggerShieldEvent onTriggerShield = new TriggerShieldEvent();

    public class TriggerShieldEvent : CustomEvent<object, ShieldData> { }
}




