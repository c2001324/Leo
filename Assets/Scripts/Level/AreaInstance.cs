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


    public RoomInstance curRoom { get; private set; }

    public LevelInstance level { get; private set; }

    public Vector2 size { get; private set; }

    public bool isEnterArea { get; private set; }

    List<RoomInstance> m_Rooms = new List<RoomInstance>();

    public RoomInstance GetRoomInstance(int index)
    {
        foreach (RoomInstance room in m_Rooms)
        {
            if (room.index == index)
            {
                return room;
            }
        }
        return null;
    }

    #region 区域初始化和销毁
    public bool Initialize(LevelInstance level, AreaData data)
    {
        this.level = level;
        m_AreaData = data;
        isEnterArea = false;
        return true;
    }

    /// <summary>
    /// 只能由Level调用
    /// </summary>
    public IEnumerator BuildArea()
    {
        GameEvent.AreaEvent.FireOnBeginBuildArea(level, this);
        yield return DoBuildRoom(GetDefaultRoomData());
        GameEvent.AreaEvent.FireOnBuildAreaComplete(level, this);
    }

    /// <summary>
    /// 只能由Level调用
    /// </summary>
    public IEnumerator DestroyArea(LevelInstance level, ExitAreaType type)
    {
        if (isEnterArea)
        {
            Debug.LogError("区域还没有退出，不能直接销毁");
        }
        GameEvent.AreaEvent.FireOnBeginDestroyArea(level, this, type);
        foreach (RoomInstance room in new List<RoomInstance>(m_Rooms))
        {
            yield return DestroyRoom(room, type);
        }
        GameEvent.AreaEvent.FireOnDestroyAreaComplete(level, this, type);
        level = null;
        m_AreaData = null;
        DestroyImmediate(gameObject);
    }

    #endregion

    #region 区域进出入
    public IEnumerator EnterArea()
    {
        if (!isEnterArea)
        {
            isEnterArea = true;
            GameEvent.AreaEvent.FireOnBeforeEnterArea(level, this);
            RoomInstance room = GetRoomInstance(GetDefaultRoomData().index);
            yield return EnterRoom(room);
            GameEvent.AreaEvent.FireOnEnterArea(level, this);
        }
    }

    public IEnumerator ExitArea(ExitAreaType type)
    {
        if (isEnterArea)
        {
            isEnterArea = false;
            GameEvent.AreaEvent.FireOnBeforeExitArea(level, this, type);
            switch (type)
            {
                case ExitAreaType.EnterNextArea:
                    yield return EixtRoom(ExitRoomType.EnterNextRoom);
                    break;
                case ExitAreaType.ExitArea:
                    yield return EixtRoom(ExitRoomType.ExitRoom);
                    break;
            }
            GameEvent.AreaEvent.FireOnExitArea(level, this, type);
        }
    }
    #endregion

    #region 房间创建和销毁
    public void BuildRoom(RoomData roomData)
    {
        StartCoroutine(DoBuildRoom(roomData));
    }

    /// <summary>
    /// 调用房间
    /// </summary>
    /// <param name="targetRoom"></param>
    /// <returns></returns>
    public IEnumerator DoBuildRoom(RoomData roomData)
    {
        if (GetRoomInstance(roomData.index) != null)
        {
            Debug.LogError("房间 " + roomData.index + " 已经创建了");
        }
        else
        {
            RoomInstance room = RoomInstance.Create(this, roomData);
            m_Rooms.Add(room);
            yield return room.BuildRoom(this);
        }
    }

    public IEnumerator DestroyRoom(RoomInstance room, ExitRoomType type)
    {
        if (room != null && m_Rooms.Remove(room))
        {
            yield return room.DestroyRoom(this, type);
        }
    }

    public IEnumerator DestroyRoom(RoomInstance room, ExitAreaType type)
    {
        switch (type)
        {
            case ExitAreaType.EnterNextArea:
                yield return DestroyRoom(room, ExitRoomType.EnterNextRoom);
                break;
            case ExitAreaType.ExitArea:
                yield return DestroyRoom(room, ExitRoomType.ExitRoom);
                break;
        }
    }
    #endregion

    #region 房间进出入
    public IEnumerator EnterRoom(RoomInstance room)
    {
        if (curRoom != null)
        {
            yield return EixtRoom(ExitRoomType.EnterNextRoom);
        }
        curRoom = room;
        yield return curRoom.Enter();
    }

    public IEnumerator EixtRoom(ExitRoomType type)
    {
        yield return curRoom.Exit(type);
        curRoom = null;
    }

    #endregion


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