using UnityEngine;
using UnityEditor;
using Untility;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 关卡的通用配置
/// </summary>
public class LevelManager : Singleton<LevelManager>, ISimpleInitManager
{


    Dictionary<int, JLevelConfig> m_LevelConfigs;

    public void Initialize()
    {
        m_LevelConfigs = LoadJsonObject.CreateObjectFromResource<Dictionary<int, JLevelConfig>>("Config/LevelConfig/LevelConfig");
        foreach (int index in m_LevelConfigs.Keys)
        {
            m_LevelConfigs[index].index = index;
        }
    }

    public JLevelConfig GetLevelConfig(int index)
    {
        JLevelConfig config = null;
        m_LevelConfigs.TryGetValue(index, out config);
        return config;
    }

    public int curAreaIndex { get; private set; }

    public static LevelInstance level { get { return instance.m_CurLevel; } }

    public static AreaInstance area
    {
        get
        {
            if (level != null)
            {
                return level.area;
            }
            return null;
        }
    }

    public static RoomInstance room
    {
        get
        {
            if (area != null)
            {
                return area.room;
            }
            return null;
        }
    }

    List<LevelInstance> m_Levels = new List<LevelInstance>();

    LevelInstance m_CurLevel;

    public LevelData GenerateLevelData(int levelIndex, int seed)
    {
        JLevelConfig config = GetLevelConfig(levelIndex);
        System.Random random = new System.Random(seed);
        return LevelData.Create(config, random);
    }

    public LevelInstance GenerateLevel(int levelIndex, int seed)
    {
        LevelData levelData = GenerateLevelData(levelIndex, seed);
        if (levelData == null)
        {
            return null;
        }
        System.Random random = new System.Random(seed);
        LevelInstance level = LevelInstance.Create(levelData, random);
        if (level != null)
        {
            m_Levels.Add(level);
        }
        return level;
    }

    public IEnumerator DestroyLevel(LevelInstance level, DestroyLevelType type)
    {
        if (level != null && m_Levels.Contains(level))
        {
            if (level == m_CurLevel)
            {
                yield return ExitLevel(type);
            }
            yield return level.DestroyLevel(this, type);
            m_Levels.Remove(level);
        }
    }

    public IEnumerator EnterLevel(LevelInstance level)
    {
        if (level != null && m_CurLevel != level)
        {
            yield return ExitLevel(DestroyLevelType.NextArea);
            m_CurLevel = level;
            yield return m_CurLevel.EnterLevel(this);
        }
    }

    public IEnumerator ExitLevel(DestroyLevelType type)
    {
        if (m_CurLevel != null)
        {
            yield return m_CurLevel.ExitLevel(this, type);
            m_CurLevel = null;
        }
    }
}