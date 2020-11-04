using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExitLevelType
{
    EnterNextLevel,  //下一关卡
    ExitLevel,
}

public enum ExitAreaType
{
    EnterNextArea,
    ExitArea,
}

public enum ExitRoomType
{
    EnterNextRoom,   //下个房间
    ExitRoom    //完全退出房间
}

/// <summary>
/// 关卡的实体
/// </summary>
public class LevelInstance : MonoBehaviour
{
    public static int praticleIndex = -1;
    public static int preCombatIndex = 0;
    public static int normalLevelBeginIndex = 1;

    public static LevelInstance Create(LevelData levelData, System.Random random)
    {
        GameObject obj = new GameObject("Level " + levelData.levelName);
        LevelInstance level = obj.AddComponent<LevelInstance>();
        if (level.Initialize(levelData, random))
        {
            return level;
        }
        else
        {
            DestroyImmediate(obj);
        }
        return null;
    }

    LevelInstance() { }


    public int levelIndex { get { return m_LevelData.levelIndex; } }

    public string levelName { get { return m_LevelData.levelName; } }

    public LevelType levelType { get { return m_LevelData.levelType; } }

    LevelData m_LevelData;

    public System.Random random { get; private set; }

    public AreaInstance curArea { get; private set; }

    public bool isEnterLevel { get; private set; }

    List<AreaInstance> m_Areas = new List<AreaInstance>();

    #region 创建和销毁关卡
    bool Initialize(LevelData levelData, System.Random random)
    {
        this.random = random;
        isEnterLevel = false;
        m_LevelData = levelData;
        return true;
    }

    /// <summary>
    /// 进入关卡，只能由LevelManager调用
    /// </summary>
    /// <returns></returns>
    public IEnumerator BuildLevel()
    {
        GameEvent.LevelEvent.FireOnBeginBuildLevel(this);
        yield return DoBuildArea(GetAreaData(0));
        GameEvent.LevelEvent.FireOnBuildLevelComplete(this);
    }

    /// <summary>
    /// 只能由LevelManager调用
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerator DestroyLevel(LevelManager manager, ExitLevelType type)
    {
        if (isEnterLevel)
        {
            Debug.LogError("关卡还没有退出，不能直接销毁");
        }
        GameEvent.LevelEvent.FireOnBeginDestroyLevel(this, type);
        ExitAreaType exitAreaType = type == ExitLevelType.EnterNextLevel ? ExitAreaType.EnterNextArea : ExitAreaType.ExitArea;
        List<AreaInstance> tempList = new List<AreaInstance>(m_Areas);
        foreach (AreaInstance area in tempList)
        {
            yield return DestroyArea(area, exitAreaType);
        }
        tempList.Clear();
        GameEvent.LevelEvent.FireOnDestroyLevelComplete(this, type);
        m_LevelData = null;
        DestroyImmediate(gameObject);
    }



    #endregion

    #region 关卡进出入
    public IEnumerator EnterLevel()
    {
        if (!isEnterLevel)
        {
            isEnterLevel = true;
            GameEvent.LevelEvent.FireOnBeforeEnterLevel(this);
            yield return DoEnterArea(0);
            GameEvent.LevelEvent.FireOnEnterLevel(this);
        }
    }

    public IEnumerator ExitLevel(ExitLevelType type)
    {
        if (isEnterLevel)
        {
            isEnterLevel = false;
            GameEvent.LevelEvent.FireOnBeforeExitLevel(this, type);
            ExitAreaType exitAreaType = type == ExitLevelType.EnterNextLevel ? ExitAreaType.EnterNextArea : ExitAreaType.ExitArea;
            yield return DoExitArea(exitAreaType);
            GameEvent.LevelEvent.FireOnExitLevel(this, type);
        }
    }
    #endregion

    #region 区域创建和销毁
    public void BuildArea(AreaData areaData)
    {
        if (GetAreaInstance(areaData.index) == null)
        {
            StartCoroutine(DoBuildArea(areaData));
        }
        else
        {
            Debug.LogError("区域 " + areaData.index + " 已经创建了");
        }
    }

    IEnumerator DoBuildArea(AreaData areaData)
    {
        if (areaData != null)
        {
            AreaInstance area = AreaInstance.Create(this, areaData);
            m_Areas.Add(area);
            yield return area.BuildArea();
        }
    }

    public IEnumerator DestroyArea(AreaInstance area, ExitAreaType type)
    {
        if (area != null && m_Areas.Remove(area))
        {
            yield return area.DestroyArea(this, type);
        }
    }
    #endregion

    #region 区域进出入
    public IEnumerator DoEnterArea(int index)
    {
        AreaInstance area = GetAreaInstance(index);
        if (area != null)
        {
            yield return DoExitArea(ExitAreaType.EnterNextArea);
            curArea = area;
            yield return curArea.EnterArea();
        }
    }

    IEnumerator DoExitArea(ExitAreaType type)
    {
        if (curArea != null)
        {
            yield return curArea.ExitArea(type);
            curArea = null;
        }
    }
    #endregion

    public AreaInstance GetAreaInstance(int index)
    {
        foreach (AreaInstance area in m_Areas)
        {
            if (area.index == index)
            {
                return area;
            }
        }
        return null;
    }

    public AreaData GetAreaData(int index)
    {
        return m_LevelData.GetAreaData(index);
    }

    public AreaData GetNextAreaData()
    {
        if (curArea != null)
        {
            return GetAreaData(curArea.index + 1);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 加载区域的各个部分所占用的加载时间百分比
    /// </summary>
    class LoadLevelProgress
    {
        static public float buildMap = 0.8f;
        static public float loadContent = 0.2f;

        static public float CalBuildMapProgress(float p)
        {
            return p * buildMap;
        }

        static public float CalLoadContent(float p)
        {
            return buildMap + p * loadContent;
        }
    }

}