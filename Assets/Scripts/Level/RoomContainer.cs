using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 由RoomManager管理
/// 管理Actor和Entity，方便查看
/// </summary>
public class RoomContainer : MonoBehaviour
{
    public static RoomContainer instance { get; private set; }

    public static void Initialize()
    {
        GameObject obj = GameObject.Find("RoomContainer");
        if (obj == null)
        {
            obj = new GameObject("RoomContainer");
        }
        instance = obj.GetComponent<RoomContainer>();
        if (instance == null)
        {
            instance = obj.AddComponent<RoomContainer>();
            DontDestroyOnLoad(instance);
        }
    }

    Dictionary<int, Transform> m_RoomList = new Dictionary<int, Transform>();

    void OnCreateArea(LevelInstance level, AreaInstance area)
    {
        name = level.levelName + "( " + area.index + ")";
    }

    void OnDestroyArea(LevelInstance level, AreaInstance area, ExitLevelType type)
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        name = "RoomContainer";
        m_RoomList.Clear();
    }

    public void AddToRoom(RoomInstance room, Transform e)
    {
        if (room.area.level.levelType != LevelType.Practice)
        {
            Transform t = GetRoom(room.index);
            e.transform.parent = t;
        }
    }

    public void RemoveFromRoom(RoomInstance room, Transform e)
    {
        e.transform.parent = null;
    }

    Transform GetRoom(int index)
    {
        Transform room = null;
        if (!m_RoomList.TryGetValue(index, out room))
        {
            room = (new GameObject("RoomInstance " + index)).transform;
            room.parent = transform;
            m_RoomList.Add(index, room);
        }
        return room;
    }
}
