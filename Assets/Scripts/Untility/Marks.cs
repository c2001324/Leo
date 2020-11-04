using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

[Flags]
public enum DamageMark
{
    None = 0,
    ForbidDamageAnimation = 1 << 0,       //不触发伤害动画，同时不会打断持续施法，包含了ForbidBreakSpelling
    ForbidCheckRelation = 1 << 1,           //不检查关系
    ForbidModifyDamageByResist = 1 << 2,//伤害不能被抗性修改
    ForbidModifyDamageByShield = 1 << 3,//伤害不能被护盾修改
    ForbidModifyDamageByGain = 1 << 4,//伤害不能被伤害加深修改
    ForbidCrit = 1 << 5,                                //不触发暴击伤害
    ForbidMinDamageLimit = 1 << 6,          //最小的伤害值限制
    ForbidDodge = 1 << 7,                           //不能被闪避，必中
    ForbidBreakSpelling = 1 << 8,               //不会打断持续施法
    DisabledBlockDamage = 1 << 9,

    ForceUndead = 1 << 20,                          // 不会死亡，优先级高于 ForceKill
    ForceKill = 1 << 21,                                //强制击杀
}

/// <summary>
/// 伤害的行为标记，标记了伤害已经进行了哪种操作
/// </summary>
[Flags]
public enum DamageResultMark
{
    None = 0,
    ResistDamage = 1 << 0,  //伤害已经计算了抗性
    GainDamage = 1 << 1,  //伤害已被加深
    ShieldDamage = 1 << 2,  //已要护盾格档了
    Crit = 1 << 3,              //已暴击
    BlockDamageSuccess = 1 << 4,   //已格档了伤害
    BlockDamageFailed = 1 << 5,   //已格档了伤害
}
