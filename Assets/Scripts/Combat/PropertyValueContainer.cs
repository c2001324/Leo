/// <summary>
/// 属性容器
/// </summary>
public class PropertyValueContainer : ValueContainer<PropertyValue>
{
    protected override PropertyValue MulValue(PropertyValue t1, PropertyValue t2)
    {
        return t1 * t2;
    }

    protected override PropertyValue PlusValue(PropertyValue t1, PropertyValue t2)
    {
        return t1 + t2;
    }

    CPropertyMarkComponent m_MarkComponent;

    public override void Initialize(IValueContainerOwner owner)
    {
        base.Initialize(owner);
        m_MarkComponent = owner.GetEntity().GetCComponent<CPropertyMarkComponent>();
        if (m_MarkComponent != null)
        {
            m_MarkComponent.onPropertyMarkChanged.AddListener(OnPropertyChanged);
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        if (m_MarkComponent != null)
        {
            m_MarkComponent.onPropertyMarkChanged.RemoveListener(OnPropertyChanged);
            m_MarkComponent = null;
        }
    }

    public override PropertyValue GetRealValue(PropertyValue value)
    {
        if (m_MarkComponent != null)
        {
            return m_MarkComponent.GetProperty(value);
        }
        else
        {
            return value;
        }
    }


}
