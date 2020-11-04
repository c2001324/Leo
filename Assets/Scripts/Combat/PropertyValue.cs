using System;
using System.Text;

/// <summary>
/// 属性类型
/// </summary>
[Flags]
public enum PropertyType
{
    None = 0,
    HpMax = 1 << 0,          //最大生命值
    MovePoint = 1 << 2,      //移动速度
    ActionPriority = 1 << 3,             //行动力
    AttackRange = 1 << 4,       //攻击范围
}

public struct PropertyValue
{
    public float hpMax;          //最大生命值
    public float movePointMax;      //移动速度
    public float actionPriority;             //行动力
    public float attackRange;               //攻击范围

    public static PropertyValue zero = new PropertyValue();

    public static PropertyValue Create(PropertyType type, float value)
    {
        switch (type)
        {
            case PropertyType.HpMax:
                return new PropertyValue() { hpMax = value };
            case PropertyType.MovePoint:
                return new PropertyValue() { movePointMax = value };
            case PropertyType.ActionPriority:
                return new PropertyValue() { actionPriority = value };
            case PropertyType.AttackRange:
                return new PropertyValue() { attackRange = value };
            default:
                return zero;
        }
    }

    public void CheckValue()
    {
        hpMax = hpMax > 0 ? hpMax : 1f;
        attackRange = attackRange > 0 ? attackRange : 1f;
        movePointMax = movePointMax > 0 ? movePointMax : 0f;
        actionPriority = actionPriority > 0 ? actionPriority : 0f;
    }

    public static string GetPropertyName(PropertyType type)
    {
        switch (type)
        {
            case PropertyType.HpMax:
                return "最大生命值";
            case PropertyType.MovePoint:
                return "移动力";
            case PropertyType.ActionPriority:
                return "行动力";
            case PropertyType.AttackRange:
                return "攻击距离";
            default:
                return "";
        }

    }
    
    #region 重载运算符
    static public PropertyValue operator +(PropertyValue p1, PropertyValue p2)
    {
        return new PropertyValue()
        {
            hpMax = p1.hpMax + p2.hpMax,
            movePointMax = p1.movePointMax + p2.movePointMax,
            actionPriority = p1.actionPriority + p2.actionPriority,
            attackRange = p1.attackRange + p2.attackRange,
        };
    }

    static public PropertyValue operator -(PropertyValue p1, PropertyValue p2)
    {
        return new PropertyValue()
        {
            hpMax = p1.hpMax - p2.hpMax,
            movePointMax = p1.movePointMax - p2.movePointMax,
            actionPriority = p1.actionPriority - p2.actionPriority,
            attackRange = p1.attackRange - p2.attackRange,
        };
    }

    static public PropertyValue operator *(PropertyValue p1, PropertyValue p2)
    {
        return new PropertyValue()
        {
            hpMax = p1.hpMax * p2.hpMax,
            movePointMax = p1.movePointMax * p2.movePointMax,
            actionPriority = p1.actionPriority * p2.actionPriority,
            attackRange = p1.attackRange * p2.attackRange,
        };
    }

    static public PropertyValue operator *(PropertyValue p1, float value)
    {
        return new PropertyValue()
        {
            hpMax = p1.hpMax * value,
            movePointMax = p1.movePointMax * value,
            actionPriority = p1.actionPriority * value,
            attackRange = p1.attackRange * value,
        };
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("hpMax={0}   movePoint={1}  actionPriority={2} attackRange={3}", hpMax, movePointMax, actionPriority, attackRange);
        return builder.ToString();
    }

    #endregion
}
