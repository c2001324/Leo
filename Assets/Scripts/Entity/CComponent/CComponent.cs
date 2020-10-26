using System;

[ComponentAttribute(true)]

public abstract class CComponent : IComponent
{

    public Entity entity { get { return m_Entity; } }
    Entity m_Entity;

    public virtual bool active { get { return state == ComponentState.Inititalize; } }

    public ComponentState state { get; private set; }

    static public T CreateComponent<T>(Entity entity) where T : CComponent, new()
    {
        T component = new T();
        if (component.Initialize(entity))
        {
            return component;
        }
        else
        {
            return null;
        }
    }

    static public CComponent CreateComponent(Type type, Entity entity)
    {
        CComponent component = Activator.CreateInstance(type) as CComponent;
        if (component.Initialize(entity))
        {
            return component;
        }
        else
        {
            return null;
        }
    }

    public bool Initialize(Entity entity)
    {
        state = ComponentState.PendingInititalize;
        m_Entity = entity;
        if (OnInitialize(entity))
        {
            return true;
        }
        else
        {
            m_Entity = null;
            return false;
        }
    }

    public void InitializeComplete()
    {
        state = ComponentState.Inititalize;
        OnInitializeComplete();
    }

    /// <summary>
    /// 开始销毁组件
    /// </summary>
    public void BeginDestroy()
    {
        if (state != ComponentState.PendingDestroy || state != ComponentState.Destroy)
        {
            OnBeginDestroy();
            state = ComponentState.PendingDestroy;
        }
    }

    /// <summary>
    /// 销毁组件成功
    /// </summary>
    public void DestroyComplete()
    {
        if (state == ComponentState.PendingDestroy)
        {
            state = ComponentState.Destroy;
            OnDestroyComplete();
        }
    }


    protected virtual bool OnInitialize(Entity entity) { return true; }
    protected virtual void OnInitializeComplete() { }
    protected virtual void OnBeginDestroy() { }
    protected virtual void OnDestroyComplete() { }
}

public class ComponentAttribute : System.Attribute
{
    public ComponentAttribute(bool abstractComponent)
    {
        isAbstractComponent = abstractComponent;
    }
    //是否为抽象的抽口
    public bool isAbstractComponent = false;
}
