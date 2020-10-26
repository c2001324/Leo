using System;

/// <summary>
/// 组件无参数的事件
/// </summary>
/// <typeparam name="TParam0"></typeparam>
public abstract class CustomEvent
{

    event Action m_Call;

    public void AddListener(Action call)
    {
        m_Call -= call;
        m_Call += call;
    }

    public void InsertListener(Action call)
    {
        m_Call -= call;
        Action tempCall = m_Call;
        m_Call = null;
        m_Call += call;
        m_Call += tempCall;
    }

    public void RemoveListener(Action call)
    {
        m_Call -= call;
    }

    public void Invoke()
    {
        if (m_Call != null)
        {
            m_Call();
        }
    }

    public void ClearListener()
    {
        m_Call = null;
    }
}


/// <summary>
/// 组件一个参数的事件
/// </summary>
/// <typeparam name="TParam0"></typeparam>
public abstract class CustomEvent<TParam0>
{

    event Action<TParam0> m_Call;

    public void AddListener(Action<TParam0> call)
    {
        m_Call -= call;
        m_Call += call;
    }

    public void InsertListener(Action<TParam0> call)
    {
        m_Call -= call;
        Action<TParam0> tempCall = m_Call;
        m_Call = null;
        m_Call += call;
        m_Call += tempCall;
    }

    public void RemoveListener(Action<TParam0> call)
    {
        m_Call -= call;
    }

    public void Invoke(TParam0 param0)
    {
        if (m_Call != null)
        {
            m_Call(param0);
        }
    }

    public void ClearListener()
    {
        m_Call = null;
    }
}




/// <summary>
/// 组件两个参数的事件
/// </summary>
/// <typeparam name="TParam0"></typeparam>
public abstract class CustomEvent<TParam0, TParam1>
{

    event Action<TParam0, TParam1> m_Call;

    public void AddListener(Action<TParam0, TParam1> call)
    {
        m_Call -= call;
        m_Call += call;
    }

    public void InsertListener(Action<TParam0, TParam1> call)
    {
        m_Call -= call;
        Action<TParam0, TParam1> tempCall = m_Call;
        m_Call = null;
        m_Call += call;
        m_Call += tempCall;
    }

    public void RemoveListener(Action<TParam0, TParam1> call)
    {
        m_Call -= call;
    }

    public void Invoke(TParam0 param0, TParam1 param1)
    {
        if (m_Call != null)
        {
            m_Call(param0, param1);
        }
    }

    public void ClearListener()
    {
        m_Call = null;
    }
}



/// <summary>
/// 组件三个参数的事件
/// </summary>
/// <typeparam name="TParam0"></typeparam>
public abstract class CustomEvent<TParam0, TParam1, TParam2>
{

    event Action<TParam0, TParam1, TParam2> m_Call;

    public void AddListener(Action<TParam0, TParam1, TParam2> call)
    {
        m_Call -= call;
        m_Call += call;
    }

    public void InsertListener(Action<TParam0, TParam1, TParam2> call)
    {
        m_Call -= call;
        Action<TParam0, TParam1, TParam2> tempCall = m_Call;
        m_Call = null;
        m_Call += call;
        m_Call += tempCall;
    }

    public void RemoveListener(Action<TParam0, TParam1, TParam2> call)
    {
        m_Call -= call;
    }

    public void Invoke(TParam0 param0, TParam1 param1, TParam2 param2)
    {
        if (m_Call != null)
        {
            m_Call(param0, param1, param2);
        }
    }

    public void ClearListener()
    {
        m_Call = null;
    }
}


/// <summary>
/// 组件四个参数的事件
/// </summary>
/// <typeparam name="TParam0"></typeparam>
public abstract class CustomEvent<TParam0, TParam1, TParam2, TParam3>
{

    event Action<TParam0, TParam1, TParam2, TParam3> m_Call;

    public void AddListener(Action<TParam0, TParam1, TParam2, TParam3> call)
    {
        m_Call -= call;
        m_Call += call;
    }

    public void InsertListener(Action<TParam0, TParam1, TParam2, TParam3> call)
    {
        m_Call -= call;
        Action<TParam0, TParam1, TParam2, TParam3> tempCall = m_Call;
        m_Call = null;
        m_Call += call;
        m_Call += tempCall;
    }

    public void RemoveListener(Action<TParam0, TParam1, TParam2, TParam3> call)
    {
        m_Call -= call;
    }

    public void Invoke(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3)
    {
        if (m_Call != null)
        {
            m_Call(param0, param1, param2, param3);
        }
    }

    public void ClearListener()
    {
        m_Call = null;
    }
}
