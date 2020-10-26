using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AreaData
{
    public static AreaData Create(LevelData levelData, int index, JAreaConfig areaConfig, System.Random random)
    {
        AreaData data = new AreaData(levelData);
        if (data.Initialize(index, areaConfig, random))
        {
            return data;
        }
        else
        {
            return null;
        }
    }

    AreaData(LevelData levelData)
    {
        m_LevelData = levelData;
    }


    public int index { get; private set; }

    public string name { get; private set; }

    public int deep { get; private set; }

    int m_IndexCounter;

    LevelData m_LevelData;

    public List<RoomData> roomDatas = new List<RoomData>();

    List<JAreaConfig.JRoomConfig> m_TempRoomConfig = new List<JAreaConfig.JRoomConfig>();

    bool Initialize(int index, JAreaConfig areaConfig, System.Random random)
    {
        m_IndexCounter = 0;
        this.index = index;
        deep = areaConfig.deep.GetValue(random);
        name = areaConfig.name;
        GenerateRooms(areaConfig.roomConfigs, random);
        GenerateRoomsFromRoomPool(areaConfig.roomPoolConfigs, random);
        GenerateRoomsFromRoomType(areaConfig.roomTypeConfigs, random);
        return ConstructRoom(areaConfig);
    }

    void AddRoomConstructorData(int deep, JAreaConfig.JRoomConfig roomConfig, bool removeConfig)
    {
        RoomData roomData = RoomData.Create(m_IndexCounter++, deep, roomConfig);
        roomDatas.Add(roomData);
        if (removeConfig)
        {
            m_TempRoomConfig.Remove(roomConfig);
        }
    }

    #region 随机要生成的房间
    /// <summary>
    /// 生成必定出现的房间
    /// </summary>
    /// <param name="areaConfig"></param>
    /// <returns></returns>
    void GenerateRooms(JAreaConfig.JRandomRoomConfig[] randomRoomConfigs, System.Random random)
    {
        if (randomRoomConfigs != null)
        {
            foreach (JAreaConfig.JRandomRoomConfig config in randomRoomConfigs)
            {
                int count = 1;
                if (config.count != null)
                {
                    count = config.count.GetValue(random);
                }
                for (int i = 0; i < count; i++)
                {
                    m_TempRoomConfig.Add(config.roomConfig);
                }
            }
        }
    }

    void GenerateRoomsFromRoomPool(JAreaConfig.JRoomPoolConfig[] poolConfigs, System.Random random)
    {
        if (poolConfigs != null)
        {
            foreach (JAreaConfig.JRoomPoolConfig pool in poolConfigs)
            {
                int count = 1;
                if (pool.count != null)
                {
                    count = pool.count.GetValue(random);
                }
                if (count > pool.roomConfigs.weights.Length)
                {
                    count = pool.roomConfigs.weights.Length;
                }
                foreach (JAreaConfig.JRoomConfig roomConfig in pool.roomConfigs.GetValues(count, random))
                {
                    m_TempRoomConfig.Add(roomConfig);
                }
            }
        }
    }

    void GenerateRoomsFromRoomType(JAreaConfig.JRandomRoomTypeConfig[] roomTypeConfigs, System.Random random)
    {
        if (roomTypeConfigs != null)
        {
            bool firstTime = true;
            while (true)
            {
                foreach (JAreaConfig.JRandomRoomTypeConfig config in roomTypeConfigs)
                {
                    int count = 1;
                    if (config.count != null)
                    {
                        count = config.count.GetValue(random);
                    }
                    for (int i = 0; i < count; i++)
                    {
                        JAreaConfig.JRoomConfig roomConfig = JAreaConfig.JRoomConfig.CreateFromType(config, random);
                        m_TempRoomConfig.Add(roomConfig);
                        //保证总房间的数量不能小于总深度
                        if (!firstTime && m_TempRoomConfig.Count >= deep)
                        {
                            break;
                        }
                    }
                }
                firstTime = false;
                if (m_TempRoomConfig.Count >= deep)
                {
                    break;
                }
            }
        }
    }

    JAreaConfig.JRoomConfig GetRoomConfig(RoomType type)
    {
        foreach (JAreaConfig.JRoomConfig config in m_TempRoomConfig)
        {
            if (config.roomType == type)
            {
                return config;
            }
        }
        return null;
    }

    #endregion

    #region 生成房间的结构

    bool ConstructRoom(JAreaConfig areaConfig)
    {
        if (ConstructFirstRoom(areaConfig) && ConstructLastRoom(areaConfig))
        {
            if (m_TempRoomConfig.Count > 0)
            {
                List<int> roomIndexList = Untility.Tool.CreateRandomList(0, m_TempRoomConfig.Count - 1);
                int curDeep = 1;
                foreach (int index in roomIndexList)
                {
                    JAreaConfig.JRoomConfig roomConfig = m_TempRoomConfig[index];
                    AddRoomConstructorData(curDeep++, roomConfig, false);
                    curDeep = curDeep > deep - 1 ? 1 : curDeep;
                }
                m_TempRoomConfig.Clear();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    bool ConstructFirstRoom(JAreaConfig areaConfig)
    {
        JAreaConfig.JRoomConfig room = null;
        switch (m_LevelData.levelType)
        {
            case LevelType.Normal:
                room = GetRoomConfig(RoomType.StartRoom);
                break;
            case LevelType.Practice:
                room = GetRoomConfig(RoomType.PracticeRoom);
                break;
            case LevelType.PreCombat:
                room = GetRoomConfig(RoomType.PreCombatRoom);
                break;
        }

        if (room == null)
        {
            Debug.LogError("找不到开始的房间");
            return false;
        }
        else
        {
            AddRoomConstructorData(0, room, true);
            return true;
        }
    }

    bool ConstructLastRoom(JAreaConfig areaConfig)
    {
        if (m_LevelData.levelType == LevelType.Normal)
        {
            JAreaConfig.JRoomConfig roomConfig = GetRoomConfig(RoomType.BossRoom);
            if (roomConfig == null)
            {
                Debug.LogError("找不到boss房间");
                return false;
            }
            else
            {
                AddRoomConstructorData(deep, roomConfig, true);
                return true;
            }
        }
        else
        {
            return true;
        }
    }


    #endregion

    public RoomData GetRoomData(int roomIndex)
    {
        foreach (RoomData data in roomDatas)
        {
            if (data.index == roomIndex)
            {
                return data;
            }
        }
        return null;
    }

    public RoomData GetRoomData(RoomType roomType)
    {
        foreach (RoomData data in roomDatas)
        {
            if (data.roomType == roomType)
            {
                return data;
            }
        }
        return null;
    }

}