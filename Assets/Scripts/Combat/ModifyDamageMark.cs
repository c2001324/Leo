using System.Collections.Generic;

/// <summary>
/// 修改伤害的标记，在能在伤害刚创建的时候修改标记
/// </summary>
public class ModifyDamageMark
{
    public ModifyDamageMark()
    {

    }

    public void Destroy()
    {
        m_AddMark.Clear();
        m_RemoveMark.Clear();
    }

    /// <summary>
    /// 修改的类型
    /// </summary>
    public enum OperatorType
    {
        Add,        //添加
        Remove, //删除
    }

    public void Process(Damage damage)
    {
        var addMarkE = m_AddMark.GetEnumerator();
        while (addMarkE.MoveNext())
        {
            if (addMarkE.Current.Value.Check(damage))
            {
                damage.AddMark(addMarkE.Current.Value.mark);
            }
        }

        var removeMarkE = m_RemoveMark.GetEnumerator();
        while (removeMarkE.MoveNext())
        {
            if (removeMarkE.Current.Value.Check(damage))
            {
                damage.RemoveMark(removeMarkE.Current.Value.mark);
            }
        }
    }

   

    public void AddMark(IPropertyController effect, OperatorType type, DamageMark mark, CheckDamageDelegate condition = null)
    {
        if (mark != DamageMark.None)
        {
            if (type == OperatorType.Add && !m_AddMark.ContainsKey(effect))
            {
                m_AddMark.Add(effect, new MarkData<DamageMark>(mark, condition));
            }
            else if (type == OperatorType.Remove && !m_RemoveMark.ContainsKey(effect))
            {
                m_RemoveMark.Add(effect, new MarkData<DamageMark>(mark, condition));
            }
        }
    }

    public void RemoveMark(IPropertyController effect, OperatorType type)
    {
        if (type == OperatorType.Add)
        {
            m_AddMark.Remove(effect);
        }
        else if (type == OperatorType.Remove)
        {
            m_RemoveMark.Remove(effect);
        }
    }

    public void RemoveMark(IPropertyController key)
    {
        m_AddMark.Remove(key);
        m_RemoveMark.Remove(key);
    }

    //待添加的Mark列表
    Dictionary<IPropertyController, MarkData<DamageMark>> m_AddMark = new Dictionary<IPropertyController, MarkData<DamageMark>>();
    //待删除的Mark列表
    Dictionary<IPropertyController, MarkData<DamageMark>> m_RemoveMark = new Dictionary<IPropertyController, MarkData<DamageMark>>();

    public class MarkData<T>
    {
        public MarkData(T mark, CheckDamageDelegate condition = null)
        {
            this.mark = mark;
            m_Condition = condition;
        }

        public bool Check(Damage damage)
        {
            return m_Condition == null || m_Condition(damage);
        }

        public readonly T mark;
        CheckDamageDelegate m_Condition;
    }
}

public delegate bool CheckDamageDelegate(Damage damage);
