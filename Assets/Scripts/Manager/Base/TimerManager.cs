using System;
using System.Collections.Generic;
using UnityEngine;
using Untility;

/// <summary>
/// 计时器管理器，有改造空间
/// </summary>
public class TimerManager : MonoBehaviour, IDonotInitManager
{
    public static TimerManager instance { get; private set; }

    public static void Initialize(GameObject obj)
    {
        instance = obj.GetComponent<TimerManager>();
        if (instance == null)
        {
            instance = obj.AddComponent<TimerManager>();
        }
    }


    public delegate void TimerCallback(uint timerOid, object param);
    public delegate void RepeatCallRemoveCallback(uint timerOid);
    
    enum TimerState
    {
        PeddingForInit,
        Runing,
        PeddingForRemove
    }

    /// <summary>
    /// 延迟执行的结构体
    /// </summary>
    class TimerData
    {
        public TimerData(uint oid, TimerCallback callback, object param, float delay, float time, float repeatRate, object tag, RepeatCallRemoveCallback repeatCallRemove)
        {
            state = TimerState.PeddingForInit;
            this.oid = oid;
            this.callback = callback;
            this.param = param;
            this.delay = delay;
            this.durationTime = time;
            this.repeatRate = repeatRate;
            this.repeatCallRemove = repeatCallRemove;
            this.tag = tag;

            m_DurationTimer = 0;
            m_DelayTimerCounter = 0;
            m_RepeatRateTimerCounter = repeatRate;
        }
        public TimerState state;
        readonly public object tag;
        readonly public uint oid;
        readonly public TimerCallback callback; //回调
        readonly public RepeatCallRemoveCallback repeatCallRemove; //重复回调完成被删除后的回调
        readonly public object param;    //参数
        readonly public float delay;         //延迟时间

        readonly public float durationTime;   //时长
        readonly public float repeatRate;    //重复的频率

        float m_DelayTimerCounter;  //延迟计时器
        float m_DurationTimer;         //计时器
        float m_RepeatRateTimerCounter; //


        public bool Update(float deltaTime)
        {
            if (state == TimerState.PeddingForRemove)
            {
                return true;
            }

            bool triggerTimer = m_DelayTimerCounter >= delay;
            bool bRemoveTimer = false;
            m_DelayTimerCounter += deltaTime;
            //开始计时
            if (triggerTimer)
            {
                m_DurationTimer += deltaTime;
                m_RepeatRateTimerCounter += deltaTime;
                if (durationTime == 0 || repeatRate <= 0)
                {
                    //直接结束
                    callback(oid, param);
                    bRemoveTimer = true;
                }
                else
                {
                    if (m_RepeatRateTimerCounter >= repeatRate)
                    {
                        callback(oid, param);
                        m_RepeatRateTimerCounter = 0f;
                    }

                    bRemoveTimer = durationTime > 0 && m_DurationTimer >= durationTime;
                    if (bRemoveTimer && repeatCallRemove != null)
                    {
                        repeatCallRemove(oid);
                    }
                }
            }

            if (bRemoveTimer)
            {
                state = TimerState.PeddingForRemove;
            }
            return bRemoveTimer;
        }
    }

    Dictionary<uint, TimerData> m_TimerList = new Dictionary<uint, TimerData>();

    List<TimerData> m_PeddingInitTimerList = new List<TimerData>();

    /// <summary>
    /// 计时器到时自动删除的Cache
    /// </summary>
    List<uint> m_PeddingRemoveTimerList = new List<uint>();

    private void FixedUpdate()
    {
        //添加等待初始化的计时器
        InitTimer();
        //运行
        UpdateTimer();
        //删除计时器
        RemoveTimer();
    }

    void InitTimer()
    {
        foreach (TimerData timer in m_PeddingInitTimerList)
        {
            if (timer.state == TimerState.PeddingForInit)
            {
                timer.state = TimerState.Runing;
                m_TimerList.Add(timer.oid, timer);
            }
        }
        m_PeddingInitTimerList.Clear();
    }

    void UpdateTimer()
    {
        foreach (TimerData timer in m_TimerList.Values)
        {
            if (timer.state == TimerState.Runing)
            {
                try
                {
                    if (timer.Update(Time.fixedDeltaTime))
                    {
                        m_PeddingRemoveTimerList.Add(timer.oid);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + "\n" + e.StackTrace);
                    timer.state = TimerState.PeddingForRemove;
                    m_PeddingRemoveTimerList.Add(timer.oid);
                }
            }
            else if (timer.state == TimerState.PeddingForRemove)
            {
                m_PeddingRemoveTimerList.Add(timer.oid);
            }
        }
    }

    void RemoveTimer()
    {
        //删除计时器
        foreach (uint oid in m_PeddingRemoveTimerList)
        {
            m_TimerList.Remove(oid);
        }
        m_PeddingRemoveTimerList.Clear();
    }


    /// <summary>
    /// 添加定时器
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="param">自定义参数</param>
    /// <param name="delay">延迟触发</param>
    /// <param name="time">持续时间，小于0时为无限</param>
    /// <param name="repeatRate">调用的频率</param>
    /// <param name="repeatCallRemoveCallback">结束后的回调</param>
    /// <returns></returns>
    public uint AddTimer(TimerCallback callback, object param, float delay, float time, float repeatRate, object tag = null, RepeatCallRemoveCallback repeatCallRemoveCallback = null)
    {
        if (callback != null && (delay > 0f || time != 0f))
        {
            uint oid = 0;
            if (time != 0f && repeatRate <= 0f)
            {
                Debug.LogError("定时器的调用频率不能小于0");
                return oid;
            }

            do
            {
                oid = Untility.GuidCreator.GetOid();
            }
            while (m_TimerList.ContainsKey(oid));

            m_PeddingInitTimerList.Add(new TimerData(oid, callback, param, delay, time, repeatRate, tag, repeatCallRemoveCallback));
            return oid;
        }
        else
        {
            return 0;
        }
    }


    /// <summary>
    /// 添加延迟器
    /// </summary>
    public uint AddDelayTimer(TimerCallback callback, object param, float delay, object tag = null)
    {
        uint oid = 0;
        do
        {
            oid = Untility.GuidCreator.GetOid();
        }
        while (m_TimerList.ContainsKey(oid));

        if (delay <= 0)
        {
            //直接执行
            callback(oid, param);
        }
        else
        {
            m_PeddingInitTimerList.Add(new TimerData(oid, callback, param, delay, 0f, 0f, tag, null));
        }
        
        return oid;
    }

    /// <summary>
    /// 取消调用
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    public bool RemoveTimer(uint oid)
    {
        TimerData data = null;
        if (m_TimerList.TryGetValue(oid, out data))
        {
            data.state = TimerState.PeddingForRemove;
            return true;
        }
        else
        {
            foreach (TimerData timer in m_PeddingInitTimerList)
            {
                if (timer.oid == oid)
                {
                    timer.state = TimerState.PeddingForRemove;
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 调用是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool ExistTimer(uint oid)
    {
        TimerData data = null;
        if (m_TimerList.TryGetValue(oid, out data))
        {
            return data.state != TimerState.PeddingForRemove;
        }
        else
        {
            return true;
        }
    }

    public bool RemoveTimer(object tag)
    {
        if (m_TimerList.Count == 0)
        {
            return false;
        }

        bool removeTimer = false;

        foreach (TimerData timer in m_TimerList.Values)
        {
            if (timer.state == TimerState.Runing && timer.tag == tag)
            {
                timer.state = TimerState.PeddingForRemove;
                removeTimer = true;
            }
        }

        foreach (TimerData timer in m_PeddingInitTimerList)
        {
            if (timer.tag != null && timer.tag == tag)
            {
                timer.state = TimerState.PeddingForRemove;
                removeTimer = true;
            }
        }
        return removeTimer;
    }
}