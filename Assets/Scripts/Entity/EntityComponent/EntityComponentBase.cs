using UnityEngine;

/// <summary>
/// Entity Mono组件的基类
/// 支持在Entity 顺序初始化
/// </summary>
public class EntityComponentBase : MonoBehaviour, IComponent
{

    public Entity entity { get; private set; }

    public virtual bool active { get { return state == ComponentState.Inititalize; } }


    public ComponentState state { get; private set; }

    public bool Initialize(Entity entity)
    {
        state = ComponentState.PendingInititalize;
        this.entity = entity;
        if (OnInitialize(entity))
        {
            return true;
        }
        else
        {
            this.entity = null;
            return false;
        }
    }

    public void InitializeComplete()
    {
        state = ComponentState.Inititalize;
        OnInitializeComplete();
    }

    public void BeginDestroy()
    {
        if (state != ComponentState.Destroy && state != ComponentState.PendingDestroy)
        {
            state = ComponentState.PendingDestroy;
            OnBeginDestroy();
            if (m_OnBeginDestroy != null)
            {
                m_OnBeginDestroy.Invoke(this);
            }
        }
    }

    public void DestroyComplete()
    {
        if (state == ComponentState.PendingDestroy)
        {
            state = ComponentState.Destroy;
            OnDestroyComplete();
        }
    }

    protected virtual bool OnInitialize(Entity e) { return true; }
    protected virtual void OnInitializeComplete() { }
    protected virtual void OnBeginDestroy() { }
    protected virtual void OnDestroyComplete() { }

    public EntityComponentEvent onBeginDestroy
    {
        get
        {
            if (m_OnBeginDestroy == null)
            {
                m_OnBeginDestroy = new EntityComponentEvent();
            }
            return m_OnBeginDestroy;
        }
    }
    EntityComponentEvent m_OnBeginDestroy;

    public class EntityComponentEvent : CustomEvent<EntityComponentBase> { }
}
