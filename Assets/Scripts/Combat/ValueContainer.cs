using System.Collections.Generic;

public interface IPropertyController { }

public interface IValueContainerOwner
{
    Entity GetEntity();
    void OnValueContainerChanged();
}

/// <summary>
/// 属性容器
/// </summary>
public abstract class ValueContainer<T> where T : struct
{

    protected Dictionary<IPropertyController, T> m_PointPropertyDict = new Dictionary<IPropertyController, T>();
    protected Dictionary<IPropertyController, T> m_PercentPropertyDict = new Dictionary<IPropertyController, T>();

    public T CalProperty(T sourceValue)
    {
        T pointValue = new T();
        foreach (T value in m_PointPropertyDict.Values)
        {
            pointValue = PlusValue(pointValue, GetRealValue(value));
        }
        pointValue = PlusValue(pointValue, sourceValue);

        T perenctValue = new T();
        foreach (T value in m_PercentPropertyDict.Values)
        {
            perenctValue = PlusValue(perenctValue, GetRealValue(value));
        }
        return PlusValue(pointValue, MulValue(pointValue, perenctValue));
    }

    protected abstract T PlusValue(T t1, T t2);

    protected abstract T MulValue(T t1, T t2);

    public IValueContainerOwner owner { get; private set; }

    public virtual void Initialize(IValueContainerOwner owner)
    {
        this.owner = owner;
    }

    public virtual void Destroy()
    {
        this.owner = null;
    }

    public abstract T GetRealValue(T value);

    public void SetProperty(IPropertyController key, T value, ValueType valueType)
    {
        if (valueType == ValueType.Point)
        {
            SetPropertyByPoint(key, value);
        }
        else if (valueType == ValueType.Percent)
        {
            SetPropertyByPercent(key, value);
        }
    }

    public bool RemoveProperty(IPropertyController key)
    {
        bool bFlag = m_PercentPropertyDict.Remove(key);
        bFlag |= m_PointPropertyDict.Remove(key);
        OnPropertyChanged();
        return bFlag;
    }

    public void SetPropertyByPoint(IPropertyController key, T value)
    {
        if (m_PointPropertyDict.ContainsKey(key))
        {
            m_PointPropertyDict[key] = value;
        }
        else
        {
            m_PointPropertyDict.Add(key, value);
        }
        OnPropertyChanged();
    }

    public void SetPropertyByPercent(IPropertyController key, T value)
    {
        if (m_PercentPropertyDict.ContainsKey(key))
        {
            m_PercentPropertyDict[key] = value;
        }
        else
        {
            m_PercentPropertyDict.Add(key, value);
        }
        OnPropertyChanged();
    }

    protected void OnPropertyChanged()
    {
        owner.OnValueContainerChanged();
    }
}
