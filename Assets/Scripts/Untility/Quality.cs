using System;
using System.Collections.Generic;

namespace Untility
{
    /// <summary>
    /// 品质
    /// </summary>
    public class Quality
    {
        static public QualityType GetNextQuality(QualityType type)
        {
            if (type != QualityType.Z)
            {
                return ++type;
            }
            else
            {
                return type;
            }
        }

        static public string ParseToString(QualityType type)
        {
            switch(type)
            {
                case QualityType.None:
                    return "无阶";
                case QualityType.D:
                    return "普通";
                case QualityType.C:
                    return "卓越";
                case QualityType.B:
                    return "稀有";
                case QualityType.A:
                    return "完美";
                case QualityType.S:
                    return "???";
                default:
                    return "";
            }
        }

        static public QualityType Parse(string str)
        {
            return (QualityType)Enum.Parse(typeof(QualityType), str);
        }

    }

    [Serializable]
    public enum QualityType
    {
        None = 0,
        D = 1,          //普通（白）
        C,          //卓越（绿）
        B,          //稀有（蓝）
        A,          //完美（紫）
        S,          //传说（金）
        Z,          //神话（橙）
    }

    public class QualityTypeComparer : IEqualityComparer<QualityType>
    {
        public bool Equals(QualityType x, QualityType y)
        {
            return x == y;
        }

        public int GetHashCode(QualityType obj)
        {
            return (int)obj;
        }
    }
}
