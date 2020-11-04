using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class TurnBase : IEquatable<TurnBase>
{
    /// <summary>
    /// 当前回合所有参与并没有死亡 的Entity
    /// </summary>
    List<CControllerComponent> m_AliveControllerList = new List<CControllerComponent>();

    public IEnumerable<CControllerComponent> readyControllerList { get { return m_ReadyControllerList; } }

    /// <summary>
    /// 还没有动行的Entity
    /// </summary>
    List<CControllerComponent> m_ReadyControllerList = new List<CControllerComponent>();

    public enum State
    {
        Ready,
        Playing,
        Complete
    }

    public State state { get; private set; }

    /// <summary>
    /// 回合数
    /// </summary>
    public int turnCounter { get; private set; }

    /// <summary>
    /// 当前正在行动的Entity
    /// </summary>
    public CControllerComponent curController { get; private set; }

    public TurnBase(int turnCounter)
    {
        this.turnCounter = turnCounter;
        state = State.Ready;
        //添加现有的Entity
        foreach (Entity entity in LevelManager.room.entities)
        {
            CControllerComponent controller = entity.GetCComponent<CControllerComponent>();
            if (controller != null)
            {
                AddToAliveController(controller);
                AddToReadyController(controller);
            }
        }
        GameEvent.EntityEvent.onEntityBorn.AddListener(OnEntityBorn);
        GameEvent.EntityEvent.onEntityDead.AddListener(OnEntityDead);
    }

    void OnEntityBorn(Entity entity)
    {
        CControllerComponent t = entity.GetCComponent<CControllerComponent>();
        if (t != null)
        {
            //当前回合出生的Entity要到下一回合才可以行动
            AddToAliveController(t);
            if (state == State.Ready)
            {
                AddToReadyController(t);
            }
        }
    }

    void OnEntityDead(Entity entity)
    {
        CControllerComponent t = entity.GetCComponent<CControllerComponent>();
        if (t != null)
        {
            RemoveFromReadyController(t);
            RemoveFromAliveController(t);
            if (t != curController)
            {
                //如果是正在行动的Entity死亡，就不做判断，因为正在行动的Entity死亡时，会自动终止自己的回合从而在下一个Entity行动开始前判断
                TurnBaseRuningState completeResult = GetTurnCompleteResult();
                if (completeResult != TurnBaseRuningState.NextEntityTurn)
                {
                    Complete(completeResult);
                }
            }
        }
    }

    /// <summary>
    /// 开始回合
    /// </summary>
    public void Start()
    {
        if (state == State.Ready)
        {
            foreach (CControllerComponent c in m_ReadyControllerList)
            {
                c.ActiveTurnBase();
            }
            state = State.Playing;
            GameEvent.TurnBaseEvent.FireOnTurnStart(this);
            NextEntityTurn();
        }
    }

    /// <summary>
    /// 结束回合
    /// </summary>
    void Complete(TurnBaseRuningState result)
    {
        if (state == State.Playing)
        {
            state = State.Complete;
            if (m_AliveControllerList.Count > 0)
            {
                List<CControllerComponent> tempList = new List<CControllerComponent>(m_AliveControllerList);
                foreach (CControllerComponent t in tempList)
                {
                    RemoveFromAliveController(t);
                }
                tempList.Clear();
            }
            m_ReadyControllerList.Clear();
            curController = null;
            GameEvent.EntityEvent.onEntityBorn.RemoveListener(OnEntityBorn);
            GameEvent.EntityEvent.onEntityDead.RemoveListener(OnEntityDead);
            GameEvent.TurnBaseEvent.FireOnTurnComplete(this, result);
            TurnBaseManager.instance.OnTurnComplete(this, result);  //单独回调，保证CombatComplete消息在TurnComplete后
        }
    }

    void NextEntityTurn()
    {
        TurnBaseRuningState completeResult = GetTurnCompleteResult();
        if (completeResult == TurnBaseRuningState.NextEntityTurn)
        {
            if (curController != null)
            {
                Debug.LogError("当前有正在行动的Entity");
            }
            else if (m_ReadyControllerList.Count > 0)
            {
                CControllerComponent target = m_ReadyControllerList[0];
                m_ReadyControllerList.RemoveAt(0);
                curController = target;
                curController.TurnStart(this);
            }
            else
            {
                Complete(TurnBaseRuningState.NextTurn);
            }
        }
        else
        {
            Complete(completeResult);
        }

    }

    /// <summary>
    /// 由CTurnBaseComponent调用
    /// </summary>
    /// <param name="target"></param>
    public void OnEntityTurnComplete(CControllerComponent controller)
    {
        if (this.curController == controller)
        {
            this.curController = null;
            NextEntityTurn();
        }
    }

    bool AddToAliveController(CControllerComponent controller)
    {
        if (controller != null && !m_AliveControllerList.Contains(controller))
        {
            m_AliveControllerList.Add(controller);
            controller.onTurnBaseValueChanged.AddListener(OnEntityTurnBaseValueChanged);
            return true;
        }
        else
        {
            return false;
        }
    }

    bool RemoveFromAliveController(CControllerComponent controller)
    {
        if (controller != null && m_AliveControllerList.Remove(controller))
        {
            controller.onTurnBaseValueChanged.RemoveListener(OnEntityTurnBaseValueChanged);
            return true;
        }
        else
        {
            return false;
        }
    }

    bool AddToReadyController(CControllerComponent controller)
    {
        if (controller != null && !m_ReadyControllerList.Contains(controller))
        {
            m_ReadyControllerList.Add(controller);
            UpdateReadControllerSort();
            return true;
        }
        else
        {
            return false;
        }
    }

    bool RemoveFromReadyController(CControllerComponent t)
    {
        if (t != null && m_ReadyControllerList.Remove(t))
        {
            UpdateReadControllerSort();
            return true;
        }
        else
        {
            return false;
        }
    }
    
    void OnEntityTurnBaseValueChanged(CControllerComponent t)
    {
        UpdateReadControllerSort();
    }

    /// <summary>
    /// 更新Entity的行动顺序
    /// </summary>
    void UpdateReadControllerSort()
    {
        m_ReadyControllerList.Sort();
    }

    /// <summary>
    /// 在每个Entity动行完成后或死亡时更新回合的状态
    /// </summary>
    TurnBaseRuningState GetTurnCompleteResult()
    {
        //总体是否还有敌人或队友
        bool hasEnemy = HasRelation(TeamId.Team_0, Relation.Enemy);
        bool hasTeamate = HasRelation(TeamId.Team_0, Relation.Teammate);

        if (hasTeamate && hasEnemy)
        {
            //有敌人和队友，继续回合
            if (curController != null || m_ReadyControllerList.Count > 0)
            {
                return TurnBaseRuningState.NextEntityTurn;
            }
            else
            {
                return TurnBaseRuningState.NextTurn;
            }
        }
        else
        {
            if (hasTeamate && !hasEnemy)
            {
                return TurnBaseRuningState.Win;
            }
            else if (!hasTeamate && hasEnemy)
            {
                return TurnBaseRuningState.Lost;
            }
            else
            {
                //默认失败
                return TurnBaseRuningState.Lost;
            }
        }
    }


    bool HasRelation(TeamId selfTeamId, Relation relation)
    {
        foreach (CControllerComponent t in m_AliveControllerList)
        {
            if (t.teamComponent.GetRelation(selfTeamId) == relation)
            {
                return true;
            }
        }
        return false;
    }

    public bool Equals(TurnBase other)
    {
        return turnCounter == other.turnCounter;
    }
}

/// <summary>
/// 回合正在运行的状态
/// </summary>
public enum TurnBaseRuningState
{
    NextTurn,     //下一回合
    NextEntityTurn, //下个Entity的回合
    Win,        //胜利
    Lost,       //失败
}