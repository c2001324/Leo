using System.Collections.Generic;
using Untility;

/// <summary>
/// 整个游戏的基础默认配置
/// </summary>
public class ConfigManager : Singleton<ConfigManager>, ISyncInitManager
{

    public IEnumerable<ManagerProgress> Initialize()
    {
        ConfigInstance.LoadConfig();
        yield return new ManagerProgress(1f, "");
    }
}

public class ConfigInstance
{
    public static void LoadConfig()
    {
        ConfigInstance config = LoadJsonObject.CreateObjectFromResource<ConfigInstance>("Config/Config");
        config.ParseToConfig();
    }

    void ParseToConfig()
    {
        Config.playerData = playerData;
        Config.system = system;
        Config.weapon = weapon;
        Config.bomb = bomb;
        Config.level = level;
        Config.combat = combat;
    }

    public PlayerData playerData;    //初始的玩家内容
    public System system;
    public Weapon weapon;
    public Bomb bomb;
    public Level level;
    public Combat combat;

    public struct PlayerData
    {
        public string roleName;
        public int weapon;
        public int weaponSkill;
        public int bomb;
        public int dashSkill;
        public int itemSkill;
    }


    public struct System
    {
        public bool loadContent;         //是否加载房间的内容
        public float baseTimeDelta;       //时间间隔
        public float minGetHitComponentSize;         //GetHitComponent 组件的最小值

        public float rayColliderY;     //进行射线碰撞测试的Y轴高度
        public float maxMoveSpeed;       //最大的移动速度
        public float minMoveSpeed;    //最小的移动速度
        public float projectileY;        //射击的默认 Y 轴坐标
        public float turnSpeed;        //转身速度
        public float strRestoreTime;   //体力回复时间
    }


    public struct Weapon
    {
        public float rateOfDamage_Jet;       //喷火器的伤害频率
        public float rateOfDamage_LaserBeam;          //射线的伤害频率
        public float projectileMaxSpeed;
    }


    public struct Bomb
    {
        public float activeTime;            //炸弹的激活时间
        public float coolDown;      //放置炸弹的CD
    }


    public struct Level
    {
        public int doorSize;        //房门大小
        public int corridorSize;    //通道大小
        public float astarGridSize; //地图模型的大小缩放
    }

    public struct Combat
    {
        
        public float hitBackTime;    //击退的时间
        public float towedSpeed;     //牵引的速度
        public float getHitTime;     //受到伤害的停止时间
        public float getHitTimeTwoHandSword;     //受到伤害的停止时间
        public float damageInterval; //受到同一个伤害的最小时间间隔
    }

}

public static class Config
{
    static public ConfigInstance.PlayerData playerData;    //初始的玩家内容
    static public ConfigInstance.System system;
    static public ConfigInstance.Weapon weapon;
    static public ConfigInstance.Bomb bomb;
    static public ConfigInstance.Level level;
    static public ConfigInstance.Combat combat;
}

