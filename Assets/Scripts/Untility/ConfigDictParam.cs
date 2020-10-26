using Newtonsoft.Json;
using System;
using System.Collections.Generic;

/// <summary>
/// 自定义参数，支持序列化
/// 只用于Json配置
/// Value 的内容必须为object才能支持多层嵌套
/// 可以引用其它ConfigDictParam作为参数
/// 可以引用Entity的属性为作参数
/// </summary>
public class ConfigDictParam : Dictionary<string, object>
{
    public ConfigDictParam()
    {

    }

    public ConfigDictParam(ConfigDictParam other)
    {
        if (other != null)
        {
            m_ReferenceParam = other.m_ReferenceParam;
            //m_ReferenceEntity = other.m_ReferenceEntity;
            var e = other.GetEnumerator();
            while (e.MoveNext())
            {
                Add(e.Current.Key, e.Current.Value);
            }
        }
    }

    /// <summary>
    /// 结合两个参数，如果有重复项，以自身的为主，不复制引用参数
    /// </summary>
    /// <param name="other"></param>
    public void Combine(ConfigDictParam other)
    {
        if (other != null)
        {
            var e = other.GetEnumerator();
            while (e.MoveNext())
            {
                if (!ContainsKey(e.Current.Key))
                {
                    Add(e.Current.Key, e.Current.Value);
                }
            }
        }
    }

    static public class Key
    {
        static readonly public string missile = "Missile";
        static readonly public string laser = "Laser";

        static readonly public string spellSound = "SpellSound";
        static readonly public string spellPraticle = "SpellPraticle";
    }

    public static char paramSign = '%';
    public static char paramDescribeStart = '[';
    public static char paramDescribeEnd = ']';

    #region 获取参数
    public string GetParamToString(string key, string defaultValue = "")
    {
        object value;
        if (TryGetValue(key, out value))
        {
            string str = value.ToString();
            if (ValueIsReferenceEntity(str))
            {
                //str = GetValueFromReferenceEntity(str);
            }
            else if (ValueIsReferenceParam(str))
            {
                str = GetValueFromReferenceParam(str);
            }
            return str;
        }
        else
        {
            return defaultValue;
        }
    }

    public bool GetParamToBoolean(string key, bool defaultValue = false)
    {
        if (HasParam(key))
        {
            return Convert.ToBoolean(GetParamToString(key));
        }
        else
        {
            return defaultValue;
        }
    }

    public float GetParamToFloat(string key, float defalutValue = 0f)
    {
        if (HasParam(key))
        {
            string value = GetParamToString(key);
            //支持50%这种类型数据
            if (value[value.Length - 1] == '%')
            {
                value = value.Remove(value.Length - 1);
                return (float)Convert.ToDouble(value) * 0.01f;
            }
            else
            {
                return (float)Convert.ToDouble(value);
            }
        }
        else
        {
            return defalutValue;
        }
    }

    public int GetParamToInt(string key, int defaultValue = 0)
    {
        if (HasParam(key))
        {
            string value = GetParamToString(key);
            return (int)Convert.ToInt64(value);
        }
        else
        {
            return defaultValue;
        }
    }

    public T GetParamToEnum<T>(string key, T defalutValue)
    {
        if (HasParam(key))
        {
            string value = GetParamToString(key);
            return (T)Enum.Parse(typeof(T), value);
        }
        else
        {
            return defalutValue;
        }
    }

    public T GetParamToEnum<T>(string key)
    {
        string value = GetParamToString(key);
        return (T)Enum.Parse(typeof(T), value);
    }


    public T GetParamToClass<T>(string key) where T : class
    {
        string value = GetParamToString(key);
        if (value == null || value == "")
        {
            return default(T);
        }
        return JsonConvert.DeserializeObject(value, typeof(T)) as T;
    }

    public T GetParamToStruct<T>(string key, T defaultValue) where T : struct
    {
        string value = GetParamToString(key);
        if (value == null || value == "")
        {
            return defaultValue;
        }
        return (T)JsonConvert.DeserializeObject(value, typeof(T));
    }

    public bool HasParam(string key)
    {
        return ContainsKey(key);
    }

    public void AddParam(string key, object value)
    {
        Add(key, value);
    }
    #endregion

    #region 引用解析的参数
    ConfigDictParam m_ReferenceParam;

    public void SetReferenceParam(ConfigDictParam reference)
    {
        m_ReferenceParam = reference;
    }

    public void ResetReferenceParam()
    {
        m_ReferenceParam = null;
    }

    bool ValueIsReferenceParam(string value)
    {
        return m_ReferenceParam != null && value.Length > 2 && value[0] == paramSign;
    }

    string GetValueFromReferenceParam(string value)
    {
        string key = value.Remove(0, 1);
        return m_ReferenceParam.GetParamToString(key);
    }
    #endregion

    #region 引用Entity相关的属性
//     Entity m_ReferenceEntity;
// 
//     public void SetReferenceEntity(Entity entity)
//     {
//         m_ReferenceEntity = entity;
//     }
// 
//     public void ResetReferenceEntity()
//     {
//         m_ReferenceEntity = null;
//     }
// 
    bool ValueIsReferenceEntity(string value)
    {
        return false;
        //return m_ReferenceEntity != null && value.Length > 2 && value[0] == paramSign && value.IndexOf(EntityParamParser.splitMark) > 0;
    }
// 
//     string GetValueFromReferenceEntity(string value)
//     {
//         string key = value.Remove(0, 1);
//         return EntityParamParser.ParserParam(key, m_ReferenceEntity);
//     }

    #endregion

    #region 用于解析参显示于描述
    Dictionary<string, DescribeDetail> m_DescribeDetails = new Dictionary<string, DescribeDetail>();

    class DescribeDetail
    {
        public ParamValueType valueType = ParamValueType.Default;
        public string customString;
    }

    public enum ParamValueType
    {
        Default,
        Percent,
        Custom
    }

    public string GetParamDescribe(string key)
    {
        DescribeDetail detail = GetParamDetail(key);
        switch (detail.valueType)
        {
            case ParamValueType.Default:
                return GetParamToString(key);
            case ParamValueType.Percent:
                float fValue = GetParamToFloat(key);
                return Untility.Tool.FloatToPercentString(fValue);
            case ParamValueType.Custom:
                return detail.customString;
            default:
                return "Error";
        }
    }

    public void SetParamDescribe(string key, ParamValueType valueType, string customString = "")
    {
        DescribeDetail detail = GetParamDetail(key);
        detail.valueType = valueType;
        detail.customString = customString;
    }

    DescribeDetail GetParamDetail(string key)
    {
        DescribeDetail detail = null;
        if (!m_DescribeDetails.TryGetValue(key, out detail))
        {
            detail = new DescribeDetail();
            m_DescribeDetails.Add(key, detail);
        }
        return detail;
    }
    #endregion
}