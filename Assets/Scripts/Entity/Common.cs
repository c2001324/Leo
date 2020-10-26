using System;

//武器大种类
[Flags]
public enum WeaponType
{
    TwoHandSword = 1 << 1,          //双手剑
    SwordShield = 1 << 2,           //剑盾
    Bow = 1 << 3,                       //弓
    Wand = 1 << 4,                     //法杖
    MagicBook = 1 << 5,               //魔法书
    All = ~(~0 << 6),
}

//武器小分类
[Flags]
public enum WeaponSubType
{
    Normal = 1 << 0,
    Charge = 1 << 1,            //充能武器
    Channel = 1 << 2,           //持续施法武器
    All = ~(~0 << 3),
}

/// <summary>
/// 武器类别 
/// </summary>
[Flags]
public enum WeaponCategories
{
    Wand = 1 << 0,                     //法杖
    MagicBook = 1 << 1,               //魔法书
    TwoHandSword = 1 << 5,          //双手剑
    All = ~(~0 << 6),
}


public enum ActiveSkillType
{
    Wind,
    Fire,
    Water,
    Earth
}

public enum SkillSpellType
{
    Simple,             //普通施法
    Channel,            //持续施法 ，不可以主动请求结束施法。只能用动画结束或者技能施法完成后结束     动画退出需要用到 AnimatorParamName.TriggerStopSpellSkill 触发器
    ManualChannel,  //手动持续施法可以主动请求结束施法
    Charge              //充能施法
}

[Flags]
public enum EntityType
{
    None = 0,
    Player = 1 << 0,
    Monster = 1 << 1,
    WeaponBuilder = 1 << 2,
    TeleportDoor = 1 << 3,
    TreasureBox = 1 << 4,
    Support = 1 << 5,
    Store = 1 << 6,
    Trap = 1 << 7,
    Unit = 1 << 8,
    All = 1 << 32,
}

public class ActorCommon
{

}

/// <summary>
/// 怪物类型
/// </summary>
public enum MonsterType
{
    Normal,
    Boss
}

/// <summary>
/// 生命值 的类型
/// </summary>
public enum HpValueType
{
    Point,
    CurHpPercent,
    MaxHpPercent
}

/// <summary>
/// 魔法值的类型
/// </summary>
public enum MpValueType
{
    Point,
    CurMpPercent,
    MaxMpPercent
}

public enum DamageGainValueType
{
    DamagePoint,
    DamagePercent,
    Custom
}

/// <summary>
/// 数值类型
/// </summary>
public enum ValueType
{
    Point,
    Percent
}

/// <summary>
/// 恢复的类型
/// </summary>
public enum RestoresType
{
    Medicine,
    Item,
    Skill, 
    Support,
    Other,
    Steal,
    All
}

public enum CompareType
{
    Equal,      //等于
    NotEqual,
    Greater,    //大于
    GreaterAndEqual,
    Less,            //小于
    LessAndEqual
}


/// <summary>
/// 驱散类型
/// </summary>
public enum PurgableType
{
    None,       //无驱散
    Normal,     //普通
    Force           //强驱散
}

public enum ModifierBuffType
{
    Buff,
    Debuff
}

[Flags]
public enum ModifierType
{
    None = 0,
    IgnoreInvulerable = 1 << 0,         //无视无敌
    Permanent = 1 << 1,                 //多个叠加在一起，持久的，每添加一个新的就刷新时间。时间到后，堆叠数量直接为0
    Stack = 1 << 2,                             //多个叠加在一起，并不刷新时间。时间到后，堆叠的个数会减少1
    Multiple = 1 << 3,                  //可以有多个独立存在
}