using UnityEngine;

public class LevelData
{
    public static LevelData Create(JLevelConfig levelConfig, System.Random random)
    {
        LevelData levelData = new LevelData();
        if (levelData.Initialize(levelConfig, random))
        {
            return levelData;
        }
        else
        {
            return null;
        }
    }

    LevelData() { }

    public int levelIndex { get; private set; }

    public string levelName { get; private set; }

    public LevelType levelType { get; private set; }

    public AreaData[] areaDatas { get; private set; }


    bool Initialize(JLevelConfig levelConfig, System.Random random)
    {
        if (levelConfig.areas == null || levelConfig.areas.Length <= 0)
        {
            Debug.LogError("关卡一定要有Area");
            return false;
        }
        levelIndex = levelConfig.index;
        levelName = levelConfig.name;
        levelType = levelConfig.levelType;

        areaDatas = new AreaData[levelConfig.areas.Length];
        //生成每区域的数据
        for (int index = 0; index < levelConfig.areas.Length; index++)
        {
            AreaData data = AreaData.Create(this, index, levelConfig.areas[index], random);
            if (data == null)
            {
                Debug.LogError("生成区域数据失败 area index = " + index);
                return false;
            }
            else
            {
                areaDatas[index] = data;
            }
        }

        return true;
    }

    public AreaData GetAreaData(int index)
    {
        if (index < 0 || index >= areaDatas.Length)
        {
            Debug.LogError("index超出范围");
            return null;
        }
        else
        {
            return areaDatas[index];
        }
    }
}