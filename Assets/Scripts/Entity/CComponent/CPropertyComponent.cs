using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 属性组件
/// </summary>
public class CPropertyComponent : CComponent, IValueContainerOwner
{
    PropertyValueContainer m_PropertyContainer;


    protected override bool OnInitialize(Entity entity)
    {
        if (base.OnInitialize(entity))
        {
            
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void OnInitializeComplete()
    {
        base.OnInitializeComplete();
        m_PropertyContainer = new PropertyValueContainer();
        m_PropertyContainer.Initialize(this);
    }

    protected override void OnBeginDestroy()
    {
        base.OnBeginDestroy();
        m_PropertyContainer.Destroy();
        m_PropertyContainer = null;
    }


    public bool alive { get { return hp > 0f; } }

    public float hp { get; private set; }

    public float hpMax { get { return property.hpMax; } }

    public float movePoint { get; private set; }

    public float movePointMax { get { return property.movePointMax; } }

    public float actionPriority { get { return property.actionPriority; } }

    public float attackRange { get { return property.attackRange; } }

    /// <summary>
    /// 总属性
    /// </summary>
    public PropertyValue property { get; private set; }

    /// <summary>
    /// 初始属性
    /// </summary>
    public PropertyValue initProperty { get; private set; }

    /// <summary>
    /// 静态属性
    /// </summary>
    public PropertyValue staticProperty { get; private set; }

    /// <summary>
    /// 属性改变后的回调
    /// </summary>
    void OnPropertyChanged()
    {
        property = m_PropertyContainer.CalProperty(initProperty + staticProperty);
        property.CheckValue();
        onPropertyChangedEvent.Invoke(this);
        SetMovePoint(movePoint);
    }

    /// <summary>
    /// 生出
    /// </summary>
    public void Born()
    {
        SetHp(hpMax);
    }

    /// <summary>
    /// 杀死
    /// </summary>
    public void Kill()
    {
    }


    public void TakeDamage(Damage damage)
    {

    }


    /// <summary>
    /// 设置生命值
    /// </summary>
    /// <param name="value"></param>
    public void SetHp(float value)
    {
        hp = value;
        hp = hp < 0f ? 0f : hp;
        hp = hp < hpMax ? hp : hpMax;
        onHpChanged.Invoke(this);
    }

    public bool CostMovePoint(float point)
    {
        if (movePoint >= point)
        {
            SetMovePoint(movePoint - point);
            return true;
        }
        else
        {
            return false;
        }
    }

    void SetMovePoint(float point)
    {
        float oldValue = movePoint;
        movePoint = point;
        movePoint = movePoint > movePointMax ? movePointMax : movePoint;
        onMovePointChanged.Invoke(this, oldValue, movePoint);
    }

    public void ResetMovePoint()
    {
        SetMovePoint(movePointMax);
    }


    #region 属性操作
   
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="value"></param>
    public void SetInitProperty(PropertyValue value)
    {
        initProperty = value;
        OnPropertyChanged();
    }

  
    public void SetProperty(IPropertyController key, ValueType valueType, PropertyValue value)
    {
        if (valueType == ValueType.Percent)
        {
            SetPropertyByPercent(key, value);
        }
        else
        {
            SetPropertyByPoint(key, value);
        }
    }

    public void SetPropertyByPoint(IPropertyController key, PropertyValue value)
    {
        float oldHpRatio = curHpRatio;
        m_PropertyContainer.SetPropertyByPoint(key, value);
        if (oldHpRatio != curHpRatio)
        {
            SetHp(oldHpRatio * hpMax);
        }
    }


    public void SetPropertyByPercent(IPropertyController key, PropertyValue value)
    {
        float oldHpRatio = curHpRatio;
        m_PropertyContainer.SetPropertyByPercent(key, value);
        if (oldHpRatio != curHpRatio)
        {
            SetHp(oldHpRatio * hpMax);
        }
    }


    public void RemoveProperty(IPropertyController key)
    {
        float oldHpRatio = curHpRatio;
        if (m_PropertyContainer.RemoveProperty(key))
        {
            if (oldHpRatio != curHpRatio)
            {
                SetHp(oldHpRatio * hpMax);
            }
        }
    }

    public void SetStaticProperty(PropertyValue value)
    {
        float oldHpRatio = curHpRatio;
        staticProperty += value;
        OnPropertyChanged();
        if (oldHpRatio != curHpRatio)
        {
            SetHp(oldHpRatio * hpMax);
        }
    }


    public void ClearStaticProperty(IPropertyController effect)
    {
        float oldHpRatio = curHpRatio;
        staticProperty = PropertyValue.zero;
        OnPropertyChanged();
        if (oldHpRatio != curHpRatio)
        {
            SetHp(oldHpRatio * hpMax);
        }
    }

    public float curHpRatio { get { return Untility.Tool.PropertyRound(hp / hpMax); } }

    #endregion

    #region IValueContainer 接口
    public Entity GetEntity()
    {
        return entity;
    }

    public void OnValueContainerChanged()
    {
        OnPropertyChanged();
    }
    #endregion


    #region 事件
    /// <summary>
    /// 出生
    /// </summary>
    public PropertyEvent onBornEvent
    {
        get
        {
            if (m_OnBornEvent == null)
            {
                m_OnBornEvent = new PropertyEvent();
            }
            return m_OnBornEvent;
        }
    }
    PropertyEvent m_OnBornEvent;


    /// <summary>
    /// 属性改变消息
    /// </summary>
    readonly public PropertyEvent onPropertyChangedEvent = new PropertyEvent();


    /// <summary>
    /// 生命值改变
    /// </summary>
    readonly public PropertyEvent onHpChanged = new PropertyEvent();

    /// <summary>
    /// 体力值改变
    /// </summary>
    readonly public StrChangedEvent onStrChangedEvent = new StrChangedEvent();


    /// <summary>
    /// 回复生命
    /// </summary>
    readonly public RestoresEvent onRestoresHp = new RestoresEvent();

    readonly public MovePointChangedEvent onMovePointChanged = new MovePointChangedEvent();


    public class PropertyEvent : CustomEvent<CPropertyComponent> { }
    public class MovePointChangedEvent : CustomEvent<CPropertyComponent, float, float> { }
    public class StrChangedEvent : CustomEvent<CPropertyComponent, int, int> { }

    public class RestoresEvent : CustomEvent<CPropertyComponent, RestoresType, float> { }
    #endregion
}

