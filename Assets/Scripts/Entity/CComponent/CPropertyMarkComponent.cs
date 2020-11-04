using System;
using System.Collections.Generic;

/// <summary>
/// 属性标记容器
/// 属性标记用来标记指定的属性是否可以被修改
/// </summary>
public class CPropertyMarkComponent : CComponent
{
    Dictionary<IPropertyController, PropertyMark> m_MarkDict = new Dictionary<IPropertyController, PropertyMark>();

    public bool AddPropertyMark(IPropertyController effect, PropertyMark mark)
    {
        if (!m_MarkDict.ContainsKey(effect))
        {
            PropertyMark oldMark = GetAllMark();
            m_MarkDict.Add(effect, mark);
            if (oldMark != GetAllMark())
            {
                onPropertyMarkChanged.Invoke();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemovePropertyMark(IPropertyController effect)
    {
        PropertyMark oldMark = GetAllMark();
        if (m_MarkDict.Remove(effect))
        {
            if (oldMark != GetAllMark())
            {
                onPropertyMarkChanged.Invoke();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 通过标记排除后，获取真正的属性值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public PropertyValue GetProperty(PropertyValue value)
    {
        PropertyMark allMark = GetAllMark();
        if (allMark == PropertyMark.None)
        {
            return value;
        }
        PropertyValue resultValue = value;
        if (CheckMark(allMark, PropertyMark.ForbidReduceHpMax))
        {
            resultValue.hpMax = value.hpMax < 0 ? 0 : value.hpMax;
        }
        return resultValue;
    }

    /// <summary>
    /// 获取所有的标记
    /// </summary>
    /// <returns></returns>
    PropertyMark GetAllMark()
    {
        PropertyMark mark = PropertyMark.None;
        foreach (PropertyMark temp in m_MarkDict.Values)
        {
            mark |= temp;
        }
        return mark;
    }


    static bool CheckMark(PropertyMark sourceMark, PropertyMark targetMark)
    {
        return (sourceMark & targetMark) == targetMark;
    }

    public readonly OnPropertyMarkChangedEvent onPropertyMarkChanged = new OnPropertyMarkChangedEvent();

    public class OnPropertyMarkChangedEvent : CustomEvent { }

}

/// <summary>
/// 属性标记
/// </summary>
[Flags]
public enum PropertyMark
{
    None = 0,

    ForbidReduceHpMax = 1 << 0,       //不会减少最大生命值
}
