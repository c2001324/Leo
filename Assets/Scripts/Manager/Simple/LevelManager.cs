using System.Collections;
using System.Collections.Generic;
using Untility;
using UnityEngine;

/// <summary>
/// 关卡的通用配置
/// </summary>
public class LevelManager : MonoBehaviour, ISimpleInitManager
{
    public static LevelManager instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType<LevelManager>();
                if (m_Instance == null)
                {
                    m_Instance = new GameObject("LevelManager").AddComponent<LevelManager>();
                }
            }
            return m_Instance;
        }
    }
    static LevelManager m_Instance;

    Dictionary<int, JLevelConfig> m_LevelConfigs;

    public void Initialize()
    {
        m_LevelConfigs = new Dictionary<int, JLevelConfig>();
        var e = LoadJsonObject.CreateObjectFromResource<Dictionary<int, string>>("Config/Level/LevelConfig").GetEnumerator();
        while (e.MoveNext())
        {
            JLevelConfig levelConfig = LoadJsonObject.CreateObjectFromResource<JLevelConfig>(e.Current.Value);
            levelConfig.index = e.Current.Key;
            m_LevelConfigs.Add(e.Current.Key, levelConfig);
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
                return level.curArea;
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
                return area.curRoom;
            }
            return null;
        }
    }

    LevelInstance m_CurLevel;

    List<LevelInstance> m_Levels = new List<LevelInstance>();

    public LevelInstance GetLevelInstance(int levelIndex)
    {
        foreach (LevelInstance level in m_Levels)
        {
            if (level.levelIndex == levelIndex)
            {
                return level;
            }
        }
        return null;
    }

    #region 创建
    public bool BuildLevel(int levelIndex, int seed)
    {
        if (GetLevelInstance(levelIndex) != null)
        {
            Debug.LogError("关卡 " + levelIndex + "已在存在");
            return false;
        }
        StartCoroutine(DoBuildLevel(levelIndex, seed));
        return true;
    }

    public void DestroyLevel(int levelIndex, ExitLevelType type)
    {
        LevelInstance level = GetLevelInstance(levelIndex);
        if (level != null)
        {
            StartCoroutine(DoDestroyLevel(level, type));
        }
    }

    IEnumerator DoBuildLevel(int levelIndex, int seed)
    {
        LevelInstance level = GenerateLevel(levelIndex, seed);
        m_Levels.Add(level);
        yield return level.BuildLevel();
    }

    LevelData GenerateLevelData(int levelIndex, int seed)
    {
        JLevelConfig config = GetLevelConfig(levelIndex);
        System.Random random = new System.Random(seed);
        return LevelData.Create(config, random);
    }

    LevelInstance GenerateLevel(int levelIndex, int seed)
    {
        LevelData levelData = GenerateLevelData(levelIndex, seed);
        if (levelData == null)
        {
            return null;
        }
        System.Random random = new System.Random(seed);
        LevelInstance level = LevelInstance.Create(levelData, random);
        return level;
    }

    IEnumerator DoDestroyLevel(LevelInstance level, ExitLevelType type)
    {
        if (level != null)
        {
            if (!level.isEnterLevel)
            {
                m_Levels.Remove(level);
                yield return level.DestroyLevel(this, type);
            }
            else
            {
                Debug.LogError("关卡还没有退出，不能直接销毁");
            }
        }
    }
    #endregion

    #region 进出入

    public bool EnterLevel(int levelIndex)
    {
        LevelInstance level = GetLevelInstance(levelIndex);
        if (level != null)
        {
            StartCoroutine(DoEnterLevel(level));
            return true;
        }
        else
        {
            Debug.LogError("还没有创建关卡 " + levelIndex);
            return false;
        }
    }

    IEnumerator DoEnterLevel(LevelInstance level)
    {
        if (m_CurLevel != null)
        {
            yield return m_CurLevel.ExitLevel(ExitLevelType.EnterNextLevel);
        }
        m_CurLevel = level;
        yield return level.EnterLevel();
    }


    public void ExitLevel(ExitLevelType type)
    {
        if (m_CurLevel != null)
        {
            StartCoroutine(DoExitLevel(type));
        }
    }

    IEnumerator DoExitLevel(ExitLevelType type)
    {
        yield return m_CurLevel.ExitLevel(type);
        m_CurLevel = null;
    }

    #endregion




    
}