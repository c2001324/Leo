using System.Collections.Generic;

/// <summary>
/// 攻击暴击
/// 每个暴击概率是单独计算的
/// 每批次的攻击，每个目标受到的伤害也是单独计算暴击的
/// </summary>
public class AttackCrit
{

    public AttackCrit()
    {

    }

    public void Destroy()
    {
        m_CritDataList.Clear();
    }

    class DamageCrit
    {
        public DamageCrit(Damage damage)
        {
            damageBatchOid = damage.batchOid;
        }
        public uint damageBatchOid;
        public bool hasTrigger = false;
        public float critValue = 0f;
    }

    DamageCrit m_DamageCrit;

    public void SetCrit(object effect, float chance, float value, CheckAttackCritHandle customHandle)
    {
        RemoveCrit(effect);
        m_CritDataList.Add(new CritData(effect, chance, value, customHandle));
        m_CritDataList.Sort((a, b) => { return b.value.CompareTo(a.value); });
    }

    public void RemoveCrit(object effect)
    {
        int index = -1;
        for (int i =0; i < m_CritDataList.Count; i++)
        {
            if (m_CritDataList[i].effect == effect)
            {
                index = i;
                break;
            }
        }
        if (index >= 0)
        {
            m_CritDataList.RemoveAt(index);
        }
    }
    
    public float GetCritValue(Damage damage)
    {
        if (m_DamageCrit == null || m_DamageCrit.damageBatchOid != damage.batchOid)
        {
            m_DamageCrit = new DamageCrit(damage);
        }
        
        if (m_DamageCrit.hasTrigger)
        {
            return m_DamageCrit.critValue;
        }
        else
        {
            m_DamageCrit.hasTrigger = true;
            foreach (CritData tempData in m_CritDataList)
            {
                if (tempData.Check(damage))
                {
                    m_DamageCrit.critValue = tempData.value;
                    break;
                }
            }

            return m_DamageCrit.critValue;
        }
    }


    public class CritData
    {
        public CritData(object effect, float chance, float value, CheckAttackCritHandle customHandle)
        {
            this.effect = effect;
            this.chance = chance;
            this.value = value;
            this.customHandle = customHandle;
        }

        public bool Check(Damage damage)
        {
            return Untility.Tool.GetRating(chance) && (customHandle == null || customHandle(damage));
        }

        public readonly object effect;
        float chance;
        public readonly float value;
        CheckAttackCritHandle customHandle;
    }

    public delegate bool CheckAttackCritHandle(Damage damage);

    List<CritData> m_CritDataList = new List<CritData>();

}




