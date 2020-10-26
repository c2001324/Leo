using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DestroyLevelType
{
    None,
    NextArea,   //下一区域
    NextLevel,  //下一关卡
    Failed,         //失败
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

    public AreaInstance area { get; private set; }

    List<AreaInstance> m_Areas = new List<AreaInstance>();

    public bool isEnterLevel { get; private set; }

    #region 初始化和销毁关卡和进出入，只能由LevelManager来调用
    bool Initialize(LevelData levelData, System.Random random)
    {
        this.random = random;
        isEnterLevel = false;
        m_LevelData = levelData;
        GameEvent.Level.FireOnBuildLevel(this);
        return true;
    }

    /// <summary>
    /// 不可以直接调用，需要经过LevelManager来调用
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerator DestroyLevel(LevelManager manager, DestroyLevelType type)
    {
        if (!isEnterLevel)
        {
            GameEvent.Level.FireOnDestroyLevel(this, type);
            //销毁区域
            foreach (AreaInstance area in m_Areas)
            {
                yield return DestroyArea(area, type, false);
            }
            m_Areas.Clear();
            m_LevelData = null;
            DestroyImmediate(gameObject);
            GameEvent.Level.FireOnDestroyLevelComplete(this, type);
        }
        else
        {
            Debug.LogError("需要先退出关卡");
        }
    }

    /// <summary>
    /// 进入关卡，只能由LevelManager调用
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnterLevel(LevelManager manager)
    {
        if (!isEnterLevel)
        {
            isEnterLevel = true;
            GameEvent.Level.FireOnBeforeEnterLevel(this);
            AreaInstance area = GetAreaInstance(0);
            yield return EnterArea(area);
            GameEvent.Level.FireOnEnterLevel(this);
        }
    }

    /// <summary>
    /// 只能由LevelManager调用
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerator ExitLevel(LevelManager manager, DestroyLevelType type)
    {
        if (isEnterLevel && area != null)
        {
            isEnterLevel = false;
            GameEvent.Level.FireOnBeforeExitLevel(this, type);
            yield return EixtArea(type);
            GameEvent.Level.FireOnExitLevel(this, type);
        }
    }

    #endregion

    #region 可以直接调用
    public AreaData GetAreaData(int index)
    {
        return m_LevelData.GetAreaData(index);
    }

    public AreaData GetNextAreaData()
    {
        if (area != null)
        {
            return GetAreaData(area.index + 1);
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

    public IEnumerator BuildArea(AreaData areaData)
    {
        AreaInstance area = GetAreaInstance(areaData.index);
        if (area == null)
        {
            area = AreaInstance.Create(this, areaData);
            if (area != null)
            {
                GameEvent.Area.FireOnBuildArea(this, area);
                //生成第一个房间
                RoomData roomData = area.GetDefaultRoomData();
                yield return area.BuildRoom(roomData);
                m_Areas.Add(area);
                GameEvent.Area.FireOnBuildAreaComplete(this, area);
            }
        }
    }

    public IEnumerator DestroyArea(AreaInstance area, DestroyLevelType type, bool remove)
    {
        if (area != null)
        {
            if (area == this.area)
            {
                yield return EixtArea(type);
            }
            GameEvent.Area.FireOnDestroyArea(this, area, type);
            yield return area.DestroyArea(this, type);
            if (remove)
            {
                m_Areas.Remove(area);
            }
            GameEvent.Area.FireOnDestroyAreaComplete(this, area, type);
        }
    }

    /// <summary>
    /// 进入区域
    /// </summary>
    /// <param name="newArea"></param>
    /// <returns></returns>
    public IEnumerator EnterArea(AreaInstance newArea)
    {
        if (newArea != null)
        {
            yield return EixtArea(DestroyLevelType.NextArea);
            area = newArea;
            yield return newArea.EnterArea(this);
        }
    }

    public IEnumerator EixtArea(DestroyLevelType type)
    {
        if (area != null)
        {
            yield return area.ExitArea(this, type);
            area = null;
        }
    }

    public AreaInstance GetAreaInstance(int areaIndex)
    {
        foreach (AreaInstance area in m_Areas)
        {
            if (area.index == areaIndex)
            {
                return area;
            }
        }
        return null;
    }
    #endregion

}