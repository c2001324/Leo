using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 区域的实体
/// </summary>
public class AreaInstance : MonoBehaviour
{
    public static AreaInstance Create(LevelInstance level, AreaData data)
    {
        GameObject obj = new GameObject("Area " + data.index);
        AreaInstance area = obj.AddComponent<AreaInstance>();
        if (area.Initialize(level, data))
        {
            area.transform.parent = level.transform;
            return area;
        }
        DestroyImmediate(obj);
        return null;
    }



    AreaInstance() { }

    public int index { get { return m_AreaData.index; } }

    new public string name { get { return m_AreaData.name; } }

    public int deep { get { return m_AreaData.deep; } }

    AreaData m_AreaData;

    public RoomInstance room;

    public LevelInstance level { get; private set; }

    public Vector2 size { get; private set; }

    List<RoomInstance> m_Rooms = new List<RoomInstance>();


    public bool isEnterArea { get; private set; }

    #region 初始化和销毁
    public bool Initialize(LevelInstance level, AreaData data)
    {
        this.level = level;
        m_AreaData = data;
        isEnterArea = false;
        return true;
    }

    /// <summary>
    /// 销毁区域
    /// </summary>
    /// <returns></returns>
    public IEnumerator DestroyArea(LevelInstance level, DestroyLevelType type)
    {
        if (!isEnterArea)
        {
            foreach (RoomInstance room in m_Rooms)
            {
                yield return DestroyRoom(room, false);
            }
            m_Rooms.Clear();
            level = null;
            m_AreaData = null;
            DestroyImmediate(gameObject);
        }
        else
        {
            Debug.LogError("需要先退出房间");
        }
    }

    /// <summary>
    /// 只能由Level调用
    /// </summary>
    public IEnumerator EnterArea(LevelInstance level)
    {
        if (!isEnterArea)
        {
            isEnterArea = true;
            GameEvent.Area.FireOnBeforeEnterArea(level, this);
            RoomInstance room = GetRoom(GetDefaultRoomData().index);
            yield return EnterRoom(room);
            GameEvent.Area.FireOnEnterArea(level, this);
        }
    }

    /// <summary>
    /// 只能由Level调用
    /// </summary>
    public IEnumerator ExitArea(LevelInstance level, DestroyLevelType type)
    {
        if (isEnterArea && room != null)
        {
            isEnterArea = false;
            GameEvent.Area.FireOnBeforeExitArea(level, this, type);
            yield return ExitRoom();
            GameEvent.Area.FireOnExitArea(level, this, type);
        }
    }

    #endregion

    #region 进出入



    /// <summary>
    /// 调用房间
    /// </summary>
    /// <param name="targetRoom"></param>
    /// <returns></returns>
    public IEnumerator EnterRoom(RoomInstance targetRoom)
    {
        if (targetRoom != null && targetRoom != room)
        {
            yield return ExitRoom();
            room = targetRoom;
            yield return targetRoom.Enter(this);
        }
    }

    public IEnumerator ExitRoom()
    {
        if (room != null)
        {
            yield return room.Exit(this);
            room = null;
        }
    }

    #endregion


    #region 加载房间

    public IEnumerator BuildRoom(RoomData roomData)
    {
        if (roomData != null)
        {
            //加载实例
            RoomInstance room = RoomInstance.Create(this, roomData);
            //加载资源
            yield return room.LoadContent();

            m_Rooms.Add(room);
        }
    }

    /// <summary>
    /// 卸载房间实例
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public IEnumerator DestroyRoom(RoomInstance room, bool remove)
    {
        yield return room.DestroyRoom(this);
        if (remove)
        {
            m_Rooms.Remove(room);
        }
    }
    #endregion

    public RoomInstance GetRoom(int index)
    {
        foreach (RoomInstance room in m_Rooms)
        {
            if (room.index == index)
            {
                return room;
            }
        }
        Debug.LogError("index = " + index + " 还没有build");
        return null;
    }

    public RoomInstance GetDefaultRoom()
    {
        RoomData roomData = GetDefaultRoomData();
        if (roomData != null)
        {
            return GetRoom(roomData.index);
        }
        else
        {
            return null;
        }
    }

    public RoomData GetRoomData(RoomType roomType)
    {
        return m_AreaData.GetRoomData(roomType);
    }

    public RoomData GetRoomData(int index)
    {
        return m_AreaData.GetRoomData(index);
    }

    public RoomData GetDefaultRoomData()
    {
        switch (level.levelType)
        {
            case LevelType.Normal:
                return GetRoomData(RoomType.StartRoom);
            case LevelType.PreCombat:
                return GetRoomData(RoomType.PreCombatRoom);
            case LevelType.Practice:
                return GetRoomData(RoomType.PracticeRoom);
            default:
                return null;
        }
    }

}