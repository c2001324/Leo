using HexMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RoomInstance : MonoBehaviour
{
    static public RoomInstance Create(AreaInstance area, RoomData roomData)
    {
        GameObject obj = new GameObject("RoomInstance " + roomData.index + " " + roomData.roomType);
        RoomInstance room = obj.AddComponent<RoomInstance>();
        if (room.Initialize(area, roomData))
        {
            return room;
        }
        return null;
    }



    RoomInstance() { }

    public int index { get { return m_Data.index; } }

    public int deep { get { return m_Data.deep; } }

    RoomData m_Data;

    public RoomType type { get { return m_Data.roomType; } }

    public AreaInstance area { get; private set; }

    public LevelInstance level { get { return area.level; } }

    public Vector3 position { get { return transform.position; } }

    public Rect rect { get; private set; }

    RoomScriptBase m_Script;

    public HexRoom hexRoom { get; private set; }

    public JRoomContentConfig contentConfig { get; private set; }

    public System.Random random { get { return area.level.random; } }

    public bool isEnterRoom { get; private set; }

    /// <summary>
    /// 房间是否可以被清空
    /// </summary>
    public bool canClear { get { return type == RoomType.MonsterRoom; } }

    /// <summary>
    /// 房间是否已清空
    /// </summary>
    public bool isComplete { get; private set; }


    #region 初始化和销毁
    public bool Initialize(AreaInstance area, RoomData data)
    {
        isEnterRoom = false;
        isComplete = false;
        transform.parent = area.transform;
        this.area = area;
        m_Data = data;
        contentConfig = RoomManager.instance.GetContentConfig(data.contentName);
        if (contentConfig == null)
        {
            Debug.LogError("找不到RoomContent " + data.contentName);
            return false;
        }

        m_Script = RoomScriptBase.Create(this, contentConfig, contentConfig.script, contentConfig.script);
        return true;
    }

    /// <summary>
    /// 只能由Area调用
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public IEnumerator BuildRoom(AreaInstance area)
    {
        GameEvent.RoomEvent.FireOnBeginBuildRoom(this);
        yield return LoadRoomModel();
        GameEvent.RoomEvent.FireOnBuildRoomComplete(this);
    }

    /// <summary>
    /// 只能由Area调用
    /// </summary>
    /// <returns></returns>
    public IEnumerator DestroyRoom(AreaInstance area, ExitRoomType type)
    {
        if (isEnterRoom)
        {
            Debug.LogError("区域还没有退出，不能直接销毁");
        }
        GameEvent.RoomEvent.FireOnBeginDestroyRoom(this);
        yield return null;
        GameEvent.RoomEvent.FireOnDestroyRoomComplete(this);
        m_Data = null;
        area = null;
        m_Script = null;
        hexRoom = null;
        DestroyImmediate(gameObject);
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="astarPath"></param>
    /// <returns></returns>
    IEnumerator LoadRoomModel()
    {
        if (m_Script != null && !m_Script.isInitialize)
        {
            //加载模型
            HexGeneratorcs generator = new HexGeneratorcs();
            GameObject movablePrefab = Resources.Load<GameObject>("HexTest/Hex0");
            GameObject[] heightPrefab = new GameObject[2];
            heightPrefab[0] = Resources.Load<GameObject>("HexTest/Hex1");
            heightPrefab[1] = Resources.Load<GameObject>("HexTest/Hex2");
            yield return generator.GenerateGrid(15, 15, transform, movablePrefab, heightPrefab);
            hexRoom = new HexRoom(generator.result.Cells);
        }
    }

    #endregion

    private void Update()
    {
        if (m_Script != null)
        {
            m_Script.Update();
        }
    }


    #region 进出入
    
    public IEnumerator Enter()
    {
        if (!isEnterRoom)
        {
            GameEvent.RoomEvent.FireOnBeforeEnterRoom(this);
            isEnterRoom = true;
            if (m_Script != null)
            {
                yield return m_Script.LoadContent(hexRoom);
                m_Script.EnterRoom();
            }
            yield return null;
            TurnBaseManager.instance.CombatStart();
            GameEvent.RoomEvent.FireOnEnterRoom(this);
        }
    }

    public IEnumerator Exit(ExitRoomType type)
    {
        if (isEnterRoom)
        {
            GameEvent.RoomEvent.FireOnBeforeExitRoom(this, type);
            isEnterRoom = false;
            if (m_Script != null)
            {
                yield return m_Script.UnloadContent();
                m_Script.ExitRoom();
            }
            if (type == ExitRoomType.ExitRoom)
            {
                PlayerManager.instance.DestoryPlayer();
            }
            yield return null;
            GameEvent.RoomEvent.FireOnExitRoom(this, type);
        }
    }

    

    /// <summary>
    /// 完成房间
    /// </summary>
    public void OnComplete()
    {
        if (!isComplete)
        {
            isComplete = true;
            GameEvent.RoomEvent.FireOnCompleteRoom(this);
            onRoomComplete.Invoke();
        }
    }
    #endregion

    #region 管理Actor
    public IEnumerable<Entity> entities { get { return m_Entities; } }

    public int entityCount { get { return m_Entities.Count; } }

    List<Entity> m_Entities = new List<Entity>();


    public Entity GetEntity(string keyName)
    {
        foreach (Entity entity in m_Entities)
        {
            if (entity.keyName == keyName)
            {
                return entity;
            }
        }
        return null;
    }

    public T GetEntity<T>(string keyName) where T : Entity
    {
        return GetEntity(keyName) as T;
    }

    public Entity GetEntity(uint oid)
    {
        foreach (Entity entity in m_Entities)
        {
            if (entity.oid == oid)
            {
                return entity;
            }
        }
        return null;
    }

    public Entity[] GetEntitys(string keyName)
    {
        List<Entity> list = new List<Entity>();
        foreach (Entity entity in m_Entities)
        {
            if (entity.keyName == keyName)
            {
                list.Add(entity);
            }
        }
        return list.ToArray();
    }

    public Entity[] GetEntitys(EntityType type)
    {
        List<Entity> list = new List<Entity>();
        foreach (Entity entity in m_Entities)
        {
            if (entity.entityType == type)
            {
                list.Add(entity);
            }
        }
        return list.ToArray();
    }

    static public T[] FindEntitiesInArea<T>(EntityType type) where T :Entity
    {
        List<T> list = new List<T>();
        foreach (Entity entity in LevelManager.room.m_Entities)
        {
            if (entity.entityType == type)
            {
                T t = entity as T;
                if (t != null)
                {
                    list.Add(t);
                }
            }
        }
        return list.ToArray();
    }

    public T[] GetEntitys<T>(EntityType type) where T : Entity
    {
        List<T> list = new List<T>();
        foreach (Entity entity in m_Entities)
        {
            if (entity.entityType == type)
            {
                T t = entity as T;
                if (t != null)
                {
                    list.Add(t);
                }
            }
        }
        return list.ToArray();
    }

    /// <summary>
    /// 获取Actor的总数
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="includePendingSpawn">是否包括正在等待出生的Actor</param>
    /// <returns></returns>
    public int GetEntityCount<T>()
    {
        int count = 0;
        Type targetType = typeof(T);
        foreach (Entity entity in m_Entities)
        {
            if (entity.GetType() == targetType)
            {
                count++;
            }
        }
        return count;
    }

  

    /// <summary>
    /// 计算房间当前的难度
    /// </summary>
    /// <returns></returns>
    public float CalDifficulty(bool includePendingSpawn = true)
    {
        return 0f;
    }

    public void AddEntity(Entity entity)
    {
        if (!m_Entities.Contains(entity))
        {
#if UNITY_EDITOR
            RoomContainer.instance.AddToRoom(this, entity.transform);
#endif
            m_Entities.Add(entity);
        }
    }

    public void RemoveEntity(Entity entity)
    {
#if UNITY_EDITOR
        RoomContainer.instance.RemoveFromRoom(this, entity.transform);
#endif
        m_Entities.Remove(entity);
    }

    public bool HasMonster()
    {
        foreach (Entity entity in m_Entities)
        {
            if (entity.entityType == EntityType.Monster)
            {
                return true;
            }
        }
        return false;
    }
    #endregion



    readonly public ClearRoomEvent onRoomComplete = new ClearRoomEvent();

    public class ClearRoomEvent : CustomEvent { }


    #region 打印信息
    public string info
    {
        get
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}{1}（深度 {2}）（内容 {3}）",
                type, index, deep, contentConfig.name);
            return builder.ToString();
        }
    }
    #endregion

}

/// <summary>
/// 房间类型
/// </summary>
[Flags]
public enum RoomType
{
    Empty = 0,
    PreCombatRoom = 1 << 0,               //准备战斗的房间
    PracticeRoom = 1 << 1,       //练习的房间
    StartRoom = 1 << 2,              //开始的房间
    BossRoom = 1 << 3,               //BOSS房间
    MonsterRoom = 1 << 4,        //普通怪物房间
    StoreRoom = 1 << 5,              //购物房间
    TreasureRoom = 1 << 6,       //宝藏房间
    WeaponBuilderRoom = 1 << 7,   //武器造房间
    SupportRoom = 1 << 8,          //补给房间
    BackRoom = 1 << 9,               //密室房间
    All = 1 << 31
}

public class RoomTypeComparer : IEqualityComparer<RoomType>
{
    public bool Equals(RoomType x, RoomType y)
    {
        return x == y;
    }

    public int GetHashCode(RoomType obj)
    {
        return (int)obj;
    }
}
