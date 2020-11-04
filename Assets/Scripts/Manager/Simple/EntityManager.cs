using System;
using System.Collections.Generic;
using UnityEngine;
using Untility;
using HexMap;

public class EntityManager : Singleton<EntityManager>, ISimpleInitManager
{

    #region 配置


    Dictionary<string, JEntityConfig> m_EntityConfig = new Dictionary<string, JEntityConfig>();

    public void Initialize()
    {
        m_EntityConfig.Clear();
        var e = LoadJsonObject.CreateObjectFromResource<Dictionary<string, JEntityConfig>>("Config/Entity/PlayerConfig");
        CopyConfig(e.Keys.GetEnumerator(), e.Values.GetEnumerator());
    }

    void CopyConfig(IEnumerator<string> keys, IEnumerator<JEntityConfig> values)
    {
        while (keys.MoveNext() && values.MoveNext())
        {
            values.Current.keyName = keys.Current;
            m_EntityConfig.Add(keys.Current, values.Current);
        }
    }

    public JEntityConfig GetEntityConfig(string keyName)
    {
        JEntityConfig config = null;
        if (keyName != null && m_EntityConfig.TryGetValue(keyName, out config))
        {
            return config;
        }
        return null;
    }

    #endregion


    #region 创建 Entity

    public T Create<T>(string keyName, RoomInstance room, HexCell cell, BaseParams param) where T : Entity
    {
        return Create(keyName, room, cell, param) as T;
    }

    public Entity Create(string keyName, RoomInstance room, HexCell cell, BaseParams param)
    {
        JEntityConfig config = GetEntityConfig(keyName);
        if (config == null)
        {
            Debug.LogError("找不到名为 " + keyName + "的EntityConfig");
            return null;
        }

        Type type = config.entityType;
        if (type == null)
        {
            Debug.LogError(keyName + " 找不到类型 " + config.type);
            return null;
        }

        EntityModel entityModel = EntityModelManager.instance.GetEntityModel(config.entityModel);
        if (entityModel == null)
        {
            Debug.LogError("找不到  " + keyName + "的EntityModel " + config.entityModel);
            return null;
        }

        HexagonsWithCenter cells = room.hexRoom.FindClearCellsInRange(config.size, cell, -1);
        if (cells == null)
        {
            Debug.LogError("找不到出生点！对象" + keyName);
            return null;
        }
        //保证Y轴

        uint oid = 0;
        do
        {
            oid = GuidCreator.GetOid();
        }
        while (m_EntityDict.ContainsKey(oid));
        Entity entity = Entity.CreateEntity(type, oid, keyName, config, entityModel, cells, param);
        if (entity != null)
        {
            m_EntityDict.Add(entity.oid, entity);
            room.AddEntity(entity);
            entity.Born(cells);
        }
        return entity;
    }

    public void DestroyEntity(Entity e, float delayDestroy = 0f)
    {
        if (e == null)
        {
            return;
        }
  
        if (e.entityState == EntityState.Alive)
        {
            e.BeginDestroy();
            if (e.entityState == EntityState.Dead)
            {
                //玩家的GameObject留在PlayerManager里删除
                if (delayDestroy >= 0)
                {
                    m_EntityPenddingForDestroyDict.Add(e.oid, new PendingForDestroyRemainingTime(delayDestroy));
                    if (!m_IsTicking)
                    {
                        m_IsTicking = true;
                        TimerManager.instance.AddTimer(OnTick, null, 0f, -1f, m_TickRate, this);
                    }
                }
            }
        }
    }

    #endregion


    Dictionary<uint, Entity> m_EntityDict = new Dictionary<uint, Entity>();

    
    Dictionary<uint, int> m_DelayDestroyEntityList = new Dictionary<uint, int>();

    Dictionary<uint, PendingForDestroyRemainingTime> m_EntityPenddingForDestroyDict = new Dictionary<uint, PendingForDestroyRemainingTime>();
    List<uint> m_EntityPendingForDestroyTempList = new List<uint>();

    class PendingForDestroyRemainingTime
    {
        public PendingForDestroyRemainingTime(float delay)
        {
            m_RemainingTime = delay;
        }

        public bool canDestory { get { return m_RemainingTime <= 0f; } }

        float m_RemainingTime;

        public void Update(float deltaTime)
        {
            m_RemainingTime -= deltaTime;
            if (m_RemainingTime < 0f)
            {
                m_RemainingTime = 0f;
            }
        }
    }

    bool m_IsTicking = false;

    readonly float m_TickRate = 0.05f;

    

    
    

    /// <summary>
    /// 设置延迟销毁标记
    /// 防止由该Entity发射出的Missile在击中目标后，该Entity已经为空了
    /// </summary>
    /// <param name="oid"></param>
    public void SetDelayDestroyMark(uint oid)
    {
        Entity e = null;
        if (m_EntityDict.TryGetValue(oid, out e) && e.entityState == EntityState.Alive)
        {
            if (m_DelayDestroyEntityList.ContainsKey(oid))
            {
                m_DelayDestroyEntityList[oid]++;
            }
            else
            {
                m_DelayDestroyEntityList.Add(oid, 1);
            }
        }
    }

    /// <summary>
    /// 删除延迟销毁标记
    /// </summary>
    /// <param name="oid"></param>
    public void RemoveDelayDestroyMark(uint oid)
    {
        if (m_DelayDestroyEntityList.ContainsKey(oid))
        {
            m_DelayDestroyEntityList[oid]--;
            if (m_DelayDestroyEntityList[oid] <= 0)
            {
                m_DelayDestroyEntityList.Remove(oid);
            }
        }
    }

    void OnTick(uint oid, object param)
    {
        //延迟删除和清空对象，因为有些对象需要延迟销毁
        if (m_EntityPenddingForDestroyDict.Count == 0)
        {
            TimerManager.instance.RemoveTimer(this);
            m_IsTicking = false;
        }
        else
        {
            foreach (KeyValuePair<uint, PendingForDestroyRemainingTime> pair in m_EntityPenddingForDestroyDict)
            {
                uint key = pair.Key;
                PendingForDestroyRemainingTime data = pair.Value;
                if (data.canDestory)
                {
                    if (!m_DelayDestroyEntityList.ContainsKey(key))
                    {
                        m_EntityPendingForDestroyTempList.Add(key);
                    }
                    else if (m_DelayDestroyEntityList[key] <= 0)
                    {
                        m_DelayDestroyEntityList.Remove(key);
                        m_EntityPendingForDestroyTempList.Add(key);
                    }
                }
                else
                {
                    data.Update(m_TickRate);
                }
            }

            foreach (uint key in m_EntityPendingForDestroyTempList)
            {
                Entity e = m_EntityDict[key];
                e.DestroyComplete();

                m_EntityDict.Remove(key);
                m_EntityPenddingForDestroyDict.Remove(key);
            }
            m_EntityPendingForDestroyTempList.Clear();
        }
    }
}