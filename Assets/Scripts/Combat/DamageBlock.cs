using System.Collections.Generic;

/// <summary>
/// 完全格挡伤害
/// </summary>
public class DamageBlock
{

    public enum BlockResult
    {
        NotBlock,
        BlockSuccess,
        BlockFailed
    }

    public DamageBlock()
    {

    }

    public void AddBlock(object key, DamageType damageType, DamageSource damageSource, CheckDamageDelegate handle)
    {
        BlockData data = new BlockData(damageType, damageSource, handle);
        if (m_BlockDatas.ContainsKey(key))
        {
            m_BlockDatas[key] = data;
        }
        else
        {
            m_BlockDatas.Add(key, data);
        }
    }

    public void RemoveBlock(object key)
    {
        m_BlockDatas.Remove(key);
    }

    public BlockResult IsBlockDamage(ref Damage damage, out object key, out float blockValue)
    {
        key = null;
        blockValue = 0f;
        float sourceDamage = damage.receiveDamageValue;
        var e = m_BlockDatas.GetEnumerator();
        while (e.MoveNext())
        {
            if (e.Current.Value.CheckDamage(damage))
            {
                if (damage.CheckMark(DamageMark.DisabledBlockDamage))
                {
                    damage.ModifyDamageByPerfectBlock(false);
                    return BlockResult.BlockFailed;
                }
                else
                {
                    damage.ModifyDamageByPerfectBlock(true);
                    key = e.Current.Key;
                    blockValue = sourceDamage - damage.receiveDamageValue;
                    return BlockResult.BlockSuccess;
                }
            }
        }
        return BlockResult.NotBlock;
    }

    Dictionary<object, BlockData> m_BlockDatas = new Dictionary<object, BlockData>();

    class BlockData
    {
        public BlockData(DamageType type, DamageSource source, CheckDamageDelegate handle)
        {
            m_Handle = handle;
            m_DamageType = type;
            m_DamageSource = source;
        }


        DamageType m_DamageType;
        DamageSource m_DamageSource;
        CheckDamageDelegate m_Handle;

        public bool CheckDamage(Damage damage)
        {
            if (Damage.IsSameDamageSource(m_DamageSource, damage.damageSource) &&
                Damage.IsSameDamageType(m_DamageType, damage.damageType))
            {
                return m_Handle == null || m_Handle(damage);
            }
            else
            {
                return false;
            }
        }
    }

}




