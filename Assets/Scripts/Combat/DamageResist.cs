using System.Collections.Generic;

/// <summary>
/// 伤害抗性
/// </summary>
public class DamageResist
{

    public DamageResist()
    {

    }

    public void AddResistValue(object effect, ValueType valueType, float value, DamageType damageType, DamageSource damageSource, CheckDamageDelegate handle)
    {
        if (valueType == ValueType.Point)
        {
            if (m_ResistPointValue.ContainsKey(effect))
            {
                m_ResistPointValue[effect] = new DamageResistValue(damageType, damageSource, value, handle);
            }
            else
            {
                m_ResistPointValue.Add(effect, new DamageResistValue(damageType, damageSource, value, handle));
            }
        }
        else
        {
            if (m_ResistPercentValue.ContainsKey(effect))
            {
                m_ResistPercentValue[effect] = new DamageResistValue(damageType, damageSource, value, handle);
            }
            else
            {
                m_ResistPercentValue.Add(effect, new DamageResistValue(damageType, damageSource, value, handle));
            }
        }
    }

    public void RemoveResistValue(object effect)
    {
        m_ResistPointValue.Remove(effect);
        m_ResistPercentValue.Remove(effect);
    }

    /// <summary>
    /// 处理
    /// </summary>
    /// <param name="damageType"></param>
    /// <param name="damageSource"></param>
    public void Process(ref Damage damage)
    {
        //计算点数
        float pointValue = 0;
        var e = m_ResistPointValue.GetEnumerator();
        while (e.MoveNext())
        {
            DamageResistValue value = e.Current.Value;
            if (value.CheckDamage(damage))
            {
                pointValue += value.value;
            }
        }
        //计算百分比
        float percentValue = 0;
        e = m_ResistPercentValue.GetEnumerator();
        while (e.MoveNext())
        {
            DamageResistValue value = e.Current.Value;
            if (value.CheckDamage(damage))
            {
                percentValue += value.value;
            }
        }

        float leftDamage = damage.receiveDamageValue - pointValue;
        if (leftDamage > 0)
        {
            leftDamage = leftDamage * (1 - percentValue);
        }
        else
        {
            leftDamage = 0;
        }
        damage.ModifyDamageByResist(leftDamage);
    }


    Dictionary<object, DamageResistValue> m_ResistPointValue = new Dictionary<object, DamageResistValue>();
    Dictionary<object, DamageResistValue> m_ResistPercentValue = new Dictionary<object, DamageResistValue>();

}


public class DamageResistValue
{
    public DamageResistValue(DamageType type, DamageSource source, float value, CheckDamageDelegate handle)
    {
        m_Value = value;
        m_Handle = handle;

        damageType = type;
        damageSource = source;
    }

    readonly public DamageType damageType;
    readonly public DamageSource damageSource;

    public float value { get { return m_Value; } }
    float m_Value;

    public bool CheckDamage(Damage damage)
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

    public void Reset()
    {
        m_Value = 0;
        m_Handle = null;
    }
}

