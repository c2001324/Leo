using System;
using System.Collections.Generic;
using UnityEngine;
using HexMap;

public enum EntityState
{
    PendingCreate,     //等待初始化
    PendingBorn,        //等待出生
    Alive,        //初始化完成
    Dead,
    Destroy                 //已经被 销毁
}

/*
 * Entity的生命流程
 * 自身初始化 OnInitialize
 * 创建组件 OnCreateComponent
 * 创建组件完成 
 * 初始化完成
 * 手动添加到房间-》自动出生，自动激活
 * 手动激活
 * 
 * 开始销毁 
 * 死亡
 * 关闭组件
 * 开始销毁组件
 * 开始销毁自身 OnBeginDestroy
 * 销毁Entity可设定延迟时间
 * 销毁组件成功
 * 销毁自身成功
 * 销毁GameObject对象
 */

[Flags]
public enum EntityFlags
{
    None = 0,
    PhysicImmune = 1 << 0,    //物理免疫状态
    MagicImmune = 1 << 1,     //魔法免疫状态
    Blind = 1 << 2,              //致盲状态，攻击100%丢失
    Invulnerable = 1 << 3,          //无敌
    Invisible = 1 << 4,          //隐身状态
    CannotFind = 1 << 7,                 //不会被查找到

    BlockDisabled = 1 << 10,   //禁用伤害减免
    EvadeDisabled = 1 << 11,   //禁用躲避
    PassiveDisabled = 1 << 12,    //禁用被动技能状态


    SpellingDisabled = 1 << 13,  //不可以施法， 不包括攻击
    Disarmed = 1 << 14,        //缴械状态
    MoveDisabled = 1 << 15,   // 禁止移动
    TurnDisabled = 1 << 16, // 禁止转向
    GetHitDisabled = 1 << 17,     //GetHit组件不会被探测到
    HitBackDisabled = 1 << 18,    //不受到击退
    ControlledDisabled = 1 << 19, //不受到控制

    Spelling = 1 << 22,      //施法中，包括施法的整个阶段    
}


/// <summary>
/// 游戏内的实体
/// 只支持 EntityComponent 组件和CComponent
/// </summary>
public abstract class Entity : MonoBehaviour
{
    static public TEntity CreateEntity<TEntity>(uint oid, string keyName, JEntityConfig config, EntityModel entityModel, HexagonsWithCenter cells, BaseParams param) where TEntity : Entity
    {
        Entity e = CreateEntity(typeof(TEntity), oid, keyName, config, entityModel, cells, param);
        if (e == null)
        {
            return null;
        }
        else
        {
            return e as TEntity;
        }
    }

    static public Entity CreateEntity(Type type, uint oid, string keyName, JEntityConfig config, EntityModel entityModel, HexagonsWithCenter cells, BaseParams param)
    {
        GameObject entityObj = new GameObject();
        EntityModel model = GameObject.Instantiate<GameObject>(entityModel.gameObject).GetComponent<EntityModel>();
        Entity entity = entityObj.AddComponent(type) as Entity;
        if (entity.Initialize(oid, keyName, config, model, cells, param))
        {
            return entity;
        }
        else
        {
            return null;
        }
    }

    #region 属性

    public override string ToString()
    {
        return this.GetType() + "_" + oid;
    }


    public Vector3 projectilePosition
    {
        get
        {
            return modelComponent.model.slot.GetBodySlot(BodySlotType.Projectile).transform.position;
        }
    }

    /// <summary>
    /// 不可以直接访问transform
    /// </summary>
    public virtual Vector3 position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }

    public Vector2 position2D { get { return new Vector2(position.x, position.z); } }


    public virtual Quaternion rotation
    {
        get
        {
            return transform.rotation;
        }
        set
        {
            transform.rotation = value;
        }
    }

    public Vector3 forward
    {
        get
        {
            return transform.forward;
        }
        set
        {
            transform.forward = value;
        }
    }

    public EntityState entityState { get; private set; }

    public string keyName { get; private set; }

    public uint oid { get; private set; } 

    public EntityModel entityModel { get { return modelComponent.model; } }

    JEntityConfig m_Config;

    public abstract EntityType entityType { get; }

    /// <summary>
    /// 每个Entity必须有一个Model
    /// </summary>
    public ModelComponent modelComponent { get; private set; }

    public HexCell centerCell { get; private set; }

    public IEnumerable<HexCell> cells { get { return m_Cells; } }
    List<HexCell> m_Cells = new List<HexCell>();

    #endregion

    #region flags

    public EntityFlags flags { get; private set; }

    Dictionary<object, EntityFlags> m_AllFlags = new Dictionary<object, EntityFlags>();

    /// <summary>
    /// 是否存在指定所有的状态
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool CheckFlags(EntityFlags state)
    {
        return CheckFlags(flags, state);
    }

    /// <summary>
    /// 是否存在其中一个状态
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool CheckFlagsByOne(EntityFlags state)
    {
        return CheckFlagsByOne(flags, state);
    }

    /// <summary>
    /// 是否存在状态
    /// </summary>
    /// <param name="sourceState"></param>
    /// <param name="targetState"></param>
    /// <returns></returns>
    public static bool CheckFlags(EntityFlags sourceState, EntityFlags targetState)
    {
        return (sourceState & targetState) == targetState;
    }

    /// <summary>
    /// 是否存在其中一个状态
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public static bool CheckFlagsByOne(EntityFlags sourceState, EntityFlags targetState)
    {
        if (targetState == EntityFlags.None)
        {
            return false;
        }
        else
        {
            return (sourceState & targetState) != EntityFlags.None;
        }
    }

    /// <summary>
    /// 获取改变的状态
    /// </summary>
    /// <param name="oldState"></param>
    /// <param name="newState"></param>
    /// <returns></returns>
    public static EntityFlags GetChangeFlags(EntityFlags oldState, EntityFlags newState)
    {
        EntityFlags s = oldState & ~newState;
        if (s == EntityFlags.None)
        {
            return newState & ~oldState;
        }
        else
        {
            return s;
        }
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    void UpdateFlags()
    {
        flags = EntityFlags.None;
        foreach (EntityFlags tempState in m_AllFlags.Values)
        {
            if (flags == EntityFlags.None)
            {
                flags = tempState;
            }
            else
            {
                flags |= tempState;
            }
        }
    }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <param name="key"></param>
    /// <param name="state"></param>
    public void AddFlags(object key, EntityFlags state)
    {
        if (state != EntityFlags.None)
        {
            EntityFlags oldState = flags;
            if (!m_AllFlags.ContainsKey(key))
            {
                m_AllFlags.Add(key, state);
            }
            else
            {
                m_AllFlags[key] = m_AllFlags[key] | state;
            }
            UpdateFlags();
            EntityFlags changeState = GetChangeFlags(oldState, flags);
            OnFlagsChanged(false, changeState);
        }
    }

    /// <summary>
    /// 删除状态
    /// </summary>
    /// <param name="key"></param>
    /// <param name="state"></param>
    public void RemoveFlags(object key, EntityFlags state)
    {
        if (state != EntityFlags.None)
        {
            EntityFlags oldState = flags;
            if (m_AllFlags.ContainsKey(key))
            {
                m_AllFlags[key] &= ~state;
                if (m_AllFlags[key] == EntityFlags.None)
                {
                    //需要测试
                    m_AllFlags.Remove(key);
                }
                UpdateFlags();
                EntityFlags changeState = GetChangeFlags(oldState, flags);
                OnFlagsChanged(false, changeState);
            }
        }
    }

    /// <summary>
    /// 删除状态
    /// </summary>
    /// <param name="key"></param>
    public void RemoveFlags(object key)
    {
        EntityFlags oldState = flags;
        if (m_AllFlags.ContainsKey(key))
        {
            m_AllFlags.Remove(key);
            UpdateFlags();
            EntityFlags changeState = GetChangeFlags(oldState, flags);
            OnFlagsChanged(false, changeState);
        }
    }

    void OnFlagsChanged(bool add, EntityFlags changedFlags)
    {
        if (changedFlags != EntityFlags.None)
        {
            if (add)
            {
                onAddFlags.Invoke(changedFlags);
            }
            else
            {
                onRemoveFlags.Invoke(changedFlags);
            }
        }
    }


    public bool IsCanControlled()
    {
        return !CheckFlags(EntityFlags.ControlledDisabled);
    }

    /// <summary>
    /// 是否可以移动
    /// </summary>
    /// <returns></returns>
    public bool IsCanMove()
    {
        return IsCanControlled() && !CheckFlags(EntityFlags.MoveDisabled);
    }

    /// <summary>
    /// 是否可以转向
    /// </summary>
    /// <returns></returns>
    public bool IsCanTurn()
    {
        return IsCanControlled() && !CheckFlags(EntityFlags.TurnDisabled);
    }

    /// <summary>
    /// 是否可以冲刺
    /// </summary>
    /// <returns></returns>
    public bool IsCanDash()
    {
        return IsCanControlled();
    }

    readonly public FlagsChangedEvent onAddFlags = new FlagsChangedEvent();
    readonly public FlagsChangedEvent onRemoveFlags = new FlagsChangedEvent();

    public class FlagsChangedEvent : CustomEvent<EntityFlags> { }

    #endregion

    #region 初始化

    public bool Initialize(uint oid, string keyName, JEntityConfig config, EntityModel entityModel, HexagonsWithCenter cells, BaseParams param)
    {
        gameObject.layer = LayerMask.NameToLayer(Layers.entity);
        this.oid = oid;
        this.keyName = keyName;

        gameObject.name = keyName + "_" + oid;

        entityState = EntityState.PendingCreate;
        m_Config = config;
        modelComponent = AddEntityComponent<ModelComponent>();
        modelComponent.SetModel(entityModel);
        if (OnInitialize(config, param) && OnCreateComponent())
        {
            CreateComponentComplete();
            entityState = EntityState.PendingBorn;
            return true;
        }
        else
        {
            Debug.LogError("初始化" + keyName + "失败");
            entityState = EntityState.Dead;
            return false;
        }
    }

    /// <summary>
    /// 只能在EntityManager或ActorManager里调用
    /// </summary>
    public void BeginDestroy()
    {
        if (entityState == EntityState.Alive)
        {
            Dead();
            RemoveCell();
            BeginDestroyComponent();
            OnBeginDestroy();
        }
    }

    /// <summary>
    /// 销毁Entity完成
    /// 由EntityManager调用
    /// </summary>
    public void DestroyComplete()
    {
        if (entityState == EntityState.Dead)
        {
            entityState = EntityState.Destroy;
            DestroyComponentCompelete();
            OnDestoryComplete();
            GameObject.DestroyImmediate(gameObject);
        }
    }

    /// <summary>
    /// 出生
    /// </summary>
    public void Born(HexagonsWithCenter cells)
    {
        if (entityState == EntityState.PendingBorn)
        {
            SetCell(cells);
            entityState = EntityState.Alive;
            GameEvent.EntityEvent.FireOnBeforeEntityBorn(this);
            OnBorn();
            GameEvent.EntityEvent.FireOnEntityBorn(this);
        }
    }

    /// <summary>
    /// 死亡
    /// </summary>
    void Dead()
    {
        if (entityState == EntityState.Alive)
        {
            entityState = EntityState.Dead;
            GameEvent.EntityEvent.FireOnBeforeEntityDead(this);
            OnDead();
            GameEvent.EntityEvent.FireOnEntityDead(this);
        }
    }


    public virtual bool ForceKill(BaseParams param = null)
    {
        EntityManager.instance.DestroyEntity(this, 0);
        return true;
    }


    #endregion

    #region cell
    public bool HasCell(HexCell cell)
    {
        return m_Cells.Contains(cell);
    }

    public void SetCell(HexagonsWithCenter cells)
    {
        RemoveCell();
        position = cells.center.entityStandPosition;
        centerCell = cells.center;
        m_Cells.AddRange(cells.allCells);
        foreach (HexCell cell in cells.allCells)
        {
            cell.SetEntity(this);
        }
    }

    public void RemoveCell()
    {
        foreach (HexCell cell in m_Cells)
        {
            cell.RemoveEntity();
        }
        m_Cells.Clear();
        centerCell = null;
    }

    /// <summary>
    /// 由HexCell回调
    /// </summary>
    /// <param name="cell"></param>
    public void OnSelectCell(HexCell cell)
    {
        modelComponent.Select(EntityModelSelectType.FromCell);
    }

    /// <summary>
    /// 由HexCell回调
    /// </summary>
    /// <param name="cell"></param>
    public void OnUnselectCell(HexCell cell)
    {
        modelComponent.UnSelect(EntityModelSelectType.FromCell);
    }

    /// <summary>
    /// 由HexCell回调
    /// </summary>
    /// <param name="cell"></param>
    public void OnLeftClickCell(HexCell cell)
    {
        modelComponent.LeftClick(EntityModelSelectType.FromCell);
    }

    /// <summary>
    /// 由HexCell回调
    /// </summary>
    /// <param name="cell"></param>
    public void OnRightClickCell(HexCell cell)
    {
        modelComponent.RightClick(EntityModelSelectType.FromCell);
    }
    #endregion

    #region modelComponent组件的回调消息
    public void OnSelect(EntityModelSelectType type)
    {
        if (centerCell != null && type != EntityModelSelectType.FromCell)
        {
            centerCell.room.OnSelectEntity(this);
        }
    }

    public void OnUnselect(EntityModelSelectType type)
    {
        if (centerCell != null && type != EntityModelSelectType.FromCell)
        {
            centerCell.room.OnUnselectEntity(this);
        }
    }

    public void OnLeftClick(EntityModelSelectType type)
    {
        if (centerCell != null && type != EntityModelSelectType.FromCell)
        {
            centerCell.room.OnLeftClickEntity(this);
        }
    }

    public void OnRightClick(EntityModelSelectType type)
    {
        if (centerCell != null && type != EntityModelSelectType.FromCell)
        {
            centerCell.room.OnRightClickEntity(this);
        }
    }
    #endregion

    public void LookAt(Entity e)
    {
        Vector3 direction = e.position - position;
        direction.y = 0;
        direction.Normalize();
        transform.forward = direction;
    }

    public void LookAt(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - position;
        direction.y = 0;
        direction.Normalize();
        transform.forward = direction;
    }

    public void LooAtByDirection(Vector3 direction)
    {
        direction.y = 0;
        direction.Normalize();
        transform.forward = direction;
    }

    public void CopyCollider(CapsuleCollider target)
    {
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CapsuleCollider>();
        }
        collider.isTrigger = target.isTrigger;
        collider.radius = target.radius;
        collider.height = target.height;
        collider.center = target.center;
    }

    public void CopyCollider(SphereCollider target)
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<SphereCollider>();
        }
        collider.isTrigger = target.isTrigger;
        collider.radius = target.radius;
        collider.center = target.center;
    }


    protected virtual void OnBorn() { }

    protected virtual void OnDead() { }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    protected abstract bool OnInitialize(JEntityConfig config, BaseParams param);

    /// <summary>
    /// 创建组件
    /// </summary>
    /// <returns></returns>
    protected abstract bool OnCreateComponent();

    /// <summary>
    /// 创建组件完成
    /// </summary>
    protected virtual void OnCreateComponentComplete() { }

    /// <summary>
    /// 开始销毁
    /// </summary>
    protected abstract void OnBeginDestroy();

    /// <summary>
    /// 销毁成功
    /// </summary>
    protected abstract void OnDestoryComplete();

    #region 组件
    Dictionary<Type, CComponent> m_ComponentMap = new Dictionary<Type, CComponent>();

    void CreateComponentComplete()
    {
        foreach (CComponent component in m_ComponentMap.Values)
        {
            component.InitializeComplete();
        }
        foreach (EntityComponentBase c in GetComponentsInChildren<EntityComponentBase>())
        {
            c.InitializeComplete();
        }
        OnCreateComponentComplete();
    }

    void BeginDestroyComponent()
    {
        foreach (CComponent component in m_ComponentMap.Values)
        {
            component.BeginDestroy();
        }
        foreach (EntityComponentBase c in GetComponentsInChildren<EntityComponentBase>())
        {
            c.BeginDestroy();
        }
    }

    void DestroyComponentCompelete()
    {
        foreach (CComponent component in m_ComponentMap.Values)
        {
            component.DestroyComplete();
        }
        m_ComponentMap.Clear();
        foreach (EntityComponentBase c in GetComponentsInChildren<EntityComponentBase>())
        {
            c.DestroyComplete();
        }
    }

    /// <summary>
    /// 添加EntityComponent组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddEntityComponent<T>() where T : EntityComponentBase
    {
        return AddEntityComponent<T>(transform);
    }

    /// <summary>
    /// 添加EntityComponent组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddEntityComponent<T>(string childName) where T : EntityComponentBase
    {
        Transform child = transform.Find(childName);
        if (child != null)
        {
            return AddEntityComponent<T>(child);
        }
        else
        {
            Debug.LogError("找不到对象 " + childName);
            return null;
        }
    }

    /// <summary>
    /// 添加EntityComponent组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddEntityComponent<T>(Transform child) where T : EntityComponentBase
    {
        T component = child.GetComponent<T>();
        if (component == null)
        {
            component = child.gameObject.AddComponent<T>();
        }

        if (component != null)
        {
            if (component.Initialize(this))
            {
                return component;
            }
            else
            {
                Debug.LogError("添加组件 " + typeof(T) + "到对象 " + this.GetType() + "初始化失败!");
                return null;
            }
        }
        else
        {
            Debug.LogError("添加组件 " + typeof(T) + "到对象 " + this.GetType() + "失败!");
            return null;
        }
    }

    public CComponent AddCComponent(Type type)
    {
        Type baseType = GetBaseType(type);
        CComponent oldComponent = null;
        if (m_ComponentMap.TryGetValue(baseType, out oldComponent))
        {
            //判断新的组件是否由旧的组件派生
            Type newType = type;
            if (oldComponent.GetType().IsAssignableFrom(newType) && oldComponent.GetType() != type)
            {
                //删除旧的组件并添加新的组件
                RemoveCComponent(oldComponent);
            }
            else
            {
                //不用添加新的组件，因为新的组件的派生组件已在添加到实体里
                return GetCComponent(type);
            }
        }

        CComponent component = CComponent.CreateComponent(type, this);
        m_ComponentMap.Add(baseType, component);
        if (entityState == EntityState.Alive)
        {
            component.InitializeComplete();
        }
        return component;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddCComponent<T>() where T : CComponent, new()
    {
        return AddCComponent(typeof(T)) as T;
    }

    Type GetBaseType(Type type)
    {
        Type curType = type;
        Type baseType = type;
        do
        {
            curType = baseType;
            baseType = curType.BaseType;
        }
        while (baseType != null && baseType != typeof(CComponent) && baseType != typeof(EntityComponentBase) && baseType != typeof(MonoBehaviour));
        //while (baseType != null && !baseType.IsAbstract);
        return curType;
    }

    public void RemoveCComponent(CComponent component)
    {
        if (component != null)
        {
            Type baseType = GetBaseType(component.GetType());
            CComponent c = null;
            if (m_ComponentMap.TryGetValue(baseType, out c))
            {
                c.BeginDestroy();
                m_ComponentMap.Remove(baseType);
            }
        }
    }

    /// <summary>
    /// 删除组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool RemoveCComponent<T>() where T : CComponent
    {
        CComponent component = GetComponent<T>();
        if (component != null)
        {
            RemoveCComponent(component);
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// 获取组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetCComponent<T>() where T : CComponent
    {
        Type baseType = GetBaseType(typeof(T));
        CComponent c = null;
        if (m_ComponentMap.TryGetValue(baseType, out c))
        {
            return c as T;
        }
        return null;
    }

    public CComponent GetCComponent(Type type)
    {
        Type baseType = GetBaseType(type);
        CComponent c = null;
        if (m_ComponentMap.TryGetValue(baseType, out c))
        {
            return c;
        }
        return null;
    }

    #endregion

}

public delegate bool CheckEntityDelegate(Entity entity);
