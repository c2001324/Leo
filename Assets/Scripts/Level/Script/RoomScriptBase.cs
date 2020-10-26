using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 房间的脚本
/// </summary>
public class RoomScriptBase
{
    public static RoomScriptBase Create(RoomInstance room, JRoomContentConfig contentConfig, string scriptName, string scriptParam)
    {
        if (scriptName == "null" || scriptName == null)
        {
            DefaultRoomScript script = new DefaultRoomScript();
            script.Initialize(room, contentConfig, scriptParam);
            return script;
        }
        else
        {
            try
            {
                Type type = Type.GetType(scriptName);
                RoomScriptBase script = Activator.CreateInstance(type) as RoomScriptBase;
                script.Initialize(room, contentConfig, scriptParam);
                return script;
            }
            catch (Exception e)
            {
                Debug.LogError("创建房间脚本" + scriptName + "失败！\n" + e.Message);
                return null;
            }
        }
    }


    public RoomInstance room { get; private set; }
    //是否已经初始化了
    public bool isInitialize { get; private set; }
    //房间资源
    public JRoomContentConfig contentConfig { get; private set; }
    //房间模型
    public RoomModel model { get; private set; }

    //是否正在加载批次
    public bool isLoadingBath { get; private set; }
    //当前的批次
    public int curBatch { get; private set; }
    //已以加载的批次
    public List<int> m_HasLoadBatch = new List<int>();


    public void Initialize(RoomInstance room, JRoomContentConfig contentConfig, string param)
    {
        this.room = room;
        isInitialize = false;
        isLoadingBath = false;
        this.room = room;
        this.contentConfig = contentConfig;
        curBatch = -1;
        OnInitialize(room, contentConfig);
    }

    protected virtual void OnInitialize(RoomInstance room, JRoomContentConfig contentConfig) { }

    protected virtual void OnDestroy() { }

    public void Destroy()
    {
        OnDestroy();
    }

    public IEnumerator LoadContent(RoomModel model)
    {
        if (!isInitialize)
        {
            this.model = model;
            if (Config.system.loadContent)
            {
                //加载默认的Entity
                foreach (RoomModelEntity.EntityData data in model.entities)
                {
                    yield return null;
                }
                //预加载的资源
                yield return LoadBatchContent(0, 0, 0, false);
                //载自定义的资源
                yield return OnLoadContent();
                //完成
                yield return 1f;
            }
            else
            {
                yield return 1f;
            }
            isInitialize = true;
        }
    }

    protected virtual IEnumerator OnLoadContent()
    {
        yield return null;
    }

    public IEnumerator UnloadContent()
    {
        if (room.entityCount == 0)
        {
            yield return null;
        }
        else
        {
            List<Entity> tempList = new List<Entity>(room.entities);
            float step = 1f / (float)tempList.Count;
            for (int i = 0; i < tempList.Count; i++)
            {
                tempList[i].ForceKill();
                yield return 0f;
            }
            yield return OnUnloadContent();
            tempList.Clear();
            yield return 1;
        }
    }

    protected virtual IEnumerator OnUnloadContent()
    {
        yield return null;
    }

    public void EnterRoom()
    {
        OnActorNumChanged();
        OnEnterRoom();
    }

    public void ExitRoom()
    {
        OnExitRoom();
    }

    protected virtual void OnEnterRoom() { }

    protected virtual void OnExitRoom() { }

    protected virtual void OnActorNumChanged()
    {
        if (!isLoadingBath && !room.isComplete && DoCheckCompleteBatch())
        {
            bool roomComplete = DoCheckRoomComplete();
            //完成当前的批次
            OnCompleteBatch();
            if (roomComplete)
            {
                OnRoomComplete();
                room.OnComplete();
            }
        }
    }


    /// <summary>
    /// 房间完成
    /// </summary>
    protected virtual void OnRoomComplete()
    {

    }

    /// <summary>
    /// 是否在下一个批次
    /// </summary>
    /// <returns></returns>
    bool HasNextBatch()
    {
        return contentConfig.GetBatchContent(curBatch + 1) != null;
    }

   

    /// <summary>
    /// 判断房间是否已被清空
    /// </summary>
    /// <returns></returns>
    protected virtual bool DoCheckCompleteBatch()
    {
        return !room.HasMonster();
    }

    /// <summary>
    /// 判断房间是否已无经完成
    /// </summary>
    /// <returns></returns>
    protected virtual bool DoCheckRoomComplete()
    {
        return !HasNextBatch();
    }

    /// <summary>
    /// 完成当前批次
    /// </summary>
    protected virtual void OnCompleteBatch()
    {

    }

    public virtual void Update()
    {

    }

    #region 加载BatchContent

    /// <summary>
    /// 加载批次的内容
    /// </summary>
    /// <param name="batch">批次</param>
    /// <param name="interval">每个Entity的生成间隔</param>
    /// <param name="delay">延迟执行</param>
    /// <param name="spawnEffect">是否有出生特效</param>
    /// <returns></returns>
    protected IEnumerator LoadBatchContent(int batch, float interval, float delay, bool spawnEffect)
    {
        if (isLoadingBath)
        {
            yield return null;
        }
        else
        {
            JRoomContentConfig.JBatchContent batchContent = contentConfig.GetBatchContent(batch);
            if (batchContent != null)
            {
                isLoadingBath = true;
                curBatch = batch;
                if (delay > 0)
                {
                    yield return new WaitForSeconds(delay);
                }

                foreach (JRoomContentConfig.JBatchContentElement e in batchContent.elements)
                {
                    if (e.type == JRoomContentConfig.SpawnPointType.Average)
                    {
                        yield return LoadBatchContentByAverage(e, interval, spawnEffect);
                    }
                    else if (e.type == JRoomContentConfig.SpawnPointType.SpawnPoint)
                    {
                        yield return LoadBatchContentBySpawnPoint(e, interval, spawnEffect);
                    }
                    else if (e.type == JRoomContentConfig.SpawnPointType.Near)
                    {
                        if (batch <= 0)
                        {
                            Debug.LogError("预加载的内容不可以使用 SpawnPointType.Near");
                        }
                        else
                        {
                            yield return LoadBatchContentByNear(e, interval, spawnEffect);
                        }
                    }
                }
                m_HasLoadBatch.Add(batch);
                isLoadingBath = false;
            }
        }
    }

    IEnumerator LoadBatchContentByAverage(JRoomContentConfig.JBatchContentElement e, float interval, bool spawnEffect)
    {
        List<string> allEntityName = e.GetAllEntityNames(room.random);
        //全房间平均
        Vector3[] positions;
        yield return null;
    }


    IEnumerator LoadBatchContentByNear(JRoomContentConfig.JBatchContentElement e, float interval, bool spawnEffect)
    {
        List<string> allEntityName = e.GetAllEntityNames(room.random);
        yield return null;
    }

    IEnumerator LoadBatchContentBySpawnPoint(JRoomContentConfig.JBatchContentElement e, float interval, bool spawnEffect)
    {
        string spawnPointName = e.GetSpawnPoint(room.random);
        RoomModelSpawnPoint.SpawnPointData spawnPoint = model.GetSpawnPoint(spawnPointName, room.random);
        if (spawnPoint == null && spawnPointName != null && spawnPointName != "")
        {
            Debug.LogError(contentConfig.name + " 在找不到出生点 " + spawnPointName);
        }
        else
        {
            yield return null;
        }
    }
    #endregion
}
