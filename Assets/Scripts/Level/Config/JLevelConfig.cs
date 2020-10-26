using UnityEngine;


public class JLevelConfig
{
    public int index;       //为0时是准备关卡
    public string name;
    public LevelType levelType;
    public JAreaConfig[] areas;
}

public enum LevelType
{
    PreCombat,
    Practice,
    Normal
}