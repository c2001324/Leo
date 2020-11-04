using System.Collections.Generic;

/// <summary>
/// 输出的伤害增益
/// </summary>
public class DamageGain
{
    public DamageGain()
    {

    }

    public void AddValue(object effect, DamageGainValueType valueType, float value, GetCustomDamageGainDelegate customGetDamageHandle, DamageType damageType, DamageSource damageSource, CheckDamageDelegate checkDamageHandle)
    {
        DamageGainValue data = new DamageGainValue(damageType, damageSource, valueType, value, customGetDamageHandle, checkDamageHandle);
        if (valueType == DamageGainValueType.DamagePoint || valueType == DamageGainValueType.Custom)
        {
            if (m_PointAndCustomValueDict.ContainsKey(effect))
            {
                m_PointAndCustomValueDict[effect] = data;
            }
            else
            {
                m_PointAndCustomValueDict.Add(effect, data);
            }
        }
        else
        {
            if (m_PercentValueDict.ContainsKey(effect))
            {
                m_PercentValueDict[effect] = data;
            }
            else
            {
                m_PercentValueDict.Add(effect, data);
            }
        }
    }

    public bool RemoveValue(object effect)
    {
        return m_PointAndCustomValueDict.Remove(effect) || m_PercentValueDict.Remove(effect);
    }

    /// <summary>
    /// 处理
    /// </summary>
    /// <param name="damageType"></param>
    /// <param name="damageSource"></param>
    public void Process(Damage damage)
    {
        //计算点数
        float pointValue = 0;
        foreach (DamageGainValue value in m_PointAndCustomValueDict.Values)
        {
            if (value.CheckDamage(damage))
            {
                pointValue += value.GetDamageValue(damage);
            }
        }
        
        //计算百分比
        float percentValue = 0;
        foreach (DamageGainValue value in m_PercentValueDict.Values)
        {
            if (value.CheckDamage(damage))
            {
                percentValue += value.GetDamageValue(damage);
            }
        }

        float totalDamage = damage.receiveDamageValue + pointValue;
        if (totalDamage > 0)
        {
            totalDamage = totalDamage * (1 + percentValue);
        }
        else
        {
            totalDamage = 0;
        }
        damage.ModifyDamageByDamageGain(totalDamage);
    }


    Dictionary<object, DamageGainValue> m_PointAndCustomValueDict = new Dictionary<object, DamageGainValue>();
    Dictionary<object, DamageGainValue> m_PercentValueDict = new Dictionary<object, DamageGainValue>();
}


public class DamageGainValue
{
    public DamageGainValue(DamageType type, DamageSource source, DamageGainValueType valueType, float value, GetCustomDamageGainDelegate customDamageHandle, CheckDamageDelegate checkDamageHandle)
    {
        m_Value = value;
        m_GainValueType = valueType;
        m_CheckDamageHandle = checkDamageHandle;
        m_GetCustomDamageValueHandle = customDamageHandle;
        m_DamageType = type;
        m_DamageSource = source;
    }

    DamageType m_DamageType;
    DamageSource m_DamageSource;
    DamageGainValueType m_GainValueType;
    float m_Value;

    public bool CheckDamage(Damage damage)
    {
        if (Damage.IsSameDamageSource(m_DamageSource, damage.damageSource) &&
            Damage.IsSameDamageType(m_DamageType, damage.damageType))
        {
            return m_CheckDamageHandle == null || m_CheckDamageHandle(damage);
        }
        else
        {
            return false;
        }
    }

    public float GetDamageValue(Damage damage)
    {
        switch (m_GainValueType)
        {
            case DamageGainValueType.DamagePoint:
                return m_Value;
            case DamageGainValueType.DamagePercent:
                return m_Value * damage.receiveDamageValue;
            case DamageGainValueType.Custom:
                return m_GetCustomDamageValueHandle(damage);
            default:
                return 0f;
        }
    }

    CheckDamageDelegate m_CheckDamageHandle;
    GetCustomDamageGainDelegate m_GetCustomDamageValueHandle;
}


public delegate float GetCustomDamageGainDelegate(Damage damage);


