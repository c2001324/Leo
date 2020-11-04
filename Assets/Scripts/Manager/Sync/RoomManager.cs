using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Untility;
using HexMap;

public class RoomManager : Singleton<RoomManager>, ISyncInitManager
{

    public IEnumerable<ManagerProgress> Initialize()
    {
#if UNITY_EDITOR
        RoomContainer.Initialize();
#endif
        LoadRoomContentConfig();
        yield return new ManagerProgress(1f, "");
    }

    #region 房间内容
    void LoadRoomContentConfig()
    {
        var e = LoadJsonObject.CreateObjectFromResource<Dictionary<string, JRoomContentConfig>>("Config/Level/RoomContentConfig").GetEnumerator();
        while (e.MoveNext())
        {
            string name = e.Current.Key.ToLower();
            e.Current.Value.name = name;
            m_SourceConfigs.Add(name, e.Current.Value);
        }
    }

    Dictionary<string, JRoomContentConfig> m_SourceConfigs = new Dictionary<string, JRoomContentConfig>();

    //存放已初始化的房间内容配置
    Dictionary<string, JRoomContentConfig> m_InitializedContentConfigs = new Dictionary<string, JRoomContentConfig>();

    /// <summary>
    /// 获取RoomContent
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public JRoomContentConfig GetContentConfig(string name)
    {
        if (name == null || name == "")
        {
            return null;
        }

        JRoomContentConfig config = null;
        if (m_InitializedContentConfigs.TryGetValue(name.ToLower(), out config))
        {
            return config;
        }
        config = InitializeContentConfig(name);
        if (config == null)
        {
            Debug.LogError("找不到RoomContent " + name);
            return null;
        }
        else
        {
            return config;
        }
    }

    /// <summary>
    /// 初始化RoomContent
    /// </summary>
    JRoomContentConfig InitializeContentConfig(string name)
    {
        name = name.ToLower();
        JRoomContentConfig config = null;
        if (m_SourceConfigs.TryGetValue(name, out config))
        {
            //已初始化后，可以在SourceConfigs里删除了
            m_SourceConfigs.Remove(name);
            JRoomContentConfig parent = GetContentConfig(config.parent);
            if (parent != null)
            {
                config.InheritFromParent(parent);
            }
            m_InitializedContentConfigs.Add(config.name, config);
            return config;
        }

        return null;
    }
    #endregion

    #region 房间类型配置
    static public bool CheckRoomType(RoomType sourceType, RoomType targetType)
    {
        if (sourceType == RoomType.Empty || targetType == RoomType.Empty)
        {
            return false;
        }
        else if (sourceType == RoomType.All || targetType == RoomType.All)
        {
            return true;
        }
        else
        {
            return (sourceType & targetType) == targetType;
        }
    }


    public static RoomType[] allType = new RoomType[] {
        RoomType.PreCombatRoom,
        RoomType.PracticeRoom,
        RoomType.StartRoom,
        RoomType.BossRoom,
        RoomType.MonsterRoom,
        RoomType.StoreRoom,
        RoomType.TreasureRoom,
        RoomType.WeaponBuilderRoom,
        RoomType.SupportRoom,
        RoomType.BackRoom
        };
    #endregion

}

public class JRoomModelConfig
{
    public bool isDirectory = false;  //是否为文件夹
    public string path;
}
