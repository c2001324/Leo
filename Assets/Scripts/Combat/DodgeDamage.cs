using System.Collections.Generic;

/// <summary>
/// 伤害避免
/// </summary>
public class DodgeDamage
{

    public DodgeDamage()
    {

    }


    public void SetDodge(object effect, float chance, DamageSource source, DamageType type, CheckDamageDelegate customHandle)
    {
        if (m_Datas.ContainsKey(effect))
        {
            m_Datas[effect] = new DodgeData(chance, source, type, customHandle);
        }
        else
        {
            m_Datas.Add(effect, new DodgeData(chance, source, type, customHandle));
        }
    }

    public void RemoveCrit(object effect)
    {
        m_Datas.Remove(effect);
    }
    
    public bool CheckIsDodgeDamage(Damage damage)
    {
        if (damage.CheckMark(DamageMark.ForbidDodge))
        {
            return false;
        }

        foreach (DodgeData data in m_Datas.Values)
        {
            if (data.Check(damage))
            {
                return true;
            }
        }
        return false;
    }


    class DodgeData
    {
        public DodgeData(float chance, DamageSource source, DamageType type,CheckDamageDelegate customHandle)
        {
            this.chance = chance;
            this.source = source;
            this.type = type;
            this.customHandle = customHandle;
        }

        public bool Check(Damage damage)
        {
            if (Damage.IsSameDamageSource(source, damage.damageSource) && Damage.IsSameDamageType(type, damage.damageType))
            {
                return Untility.Tool.GetRating(chance) && (customHandle == null || customHandle(damage));
            }
            else
            {
                return false;
            }
        }

        float chance;
        CheckDamageDelegate customHandle;
        DamageSource source;
        DamageType type;
    }

    Dictionary<object, DodgeData> m_Datas = new Dictionary<object, DodgeData>();

}




