/// <summary>
/// 基础参数，不支持序列化
/// </summary>
public class BaseParams
{
    public BaseParams(params object[] param)
    {
        this.m_Param = param;
    }

    public object[] param { get { return m_Param; } }
    object[] m_Param;

    public int paramCount
    {
        get
        {
            if (param == null)
            {
                return 0;
            }
            else
            {
                return param.Length;
            }
        }
    }

    public bool TryGetParamByIndex<T>(int index, out T result)
    {
        result = default(T);
        if (index >= param.Length)
        {
            return false;
        }
        else
        {
            try
            {
                result = (T)param[index];
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public virtual T GetParamByIndex<T>(int index)
    {
        if (param[index] != null)
        {
            return (T)param[index];
        }
        else
        {
            return default(T);
        }
    }


    public object GetParamByIndex(int index)
    {
        if (param[index] != null)
        {
            return param[index];
        }
        else
        {
            return null;
        }
    }

    public virtual void SetParamByIndex(int index, object newParam)
    {
        if (index < param.Length && index >= 0)
        {
            //添加新的参数
            param[index] = newParam;
        }
        else
        {
            UnityEngine.Debug.LogError("没有找到效果参数，index = " + index);
        }
    }

}