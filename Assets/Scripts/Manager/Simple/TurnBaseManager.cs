using System.Collections.Generic;
using UnityEngine;
using Untility;


public class TurnBaseManager : Singleton<TurnBaseManager>, ISimpleInitManager
{
    /// <summary>
    /// 是否在回合的模式里
    /// </summary>
    public bool isTurnBaseModel { get { return GameSceneManager.instance.curGameScene == GameSceneType.Practice || GameSceneManager.instance.curGameScene == GameSceneType.Combat; } }

    public enum State
    {
        Combating,          //正在战斗中
        CombatComplete  //战斗结束
    }

    public State state { get; private set; }

    
    //当前正在执行的回合
    public TurnBase curTurnBase { get; private set; }

    //下个回合
    public TurnBase nextTurnBase { get; private set; }

    public CControllerComponent curController
    {
        get
        {
            if (curTurnBase == null)
            {
                return null;
            }
            else
            {
                return curTurnBase.curController;
            }
        }
    }

    public void Initialize()
    {
        state = State.CombatComplete;
    }

    /// <summary>
    /// 战斗开始
    /// </summary>
    public void CombatStart()
    {
        if (state == State.CombatComplete)
        {
            state = State.Combating;
            GameEvent.TurnBaseEvent.FireOnCombatStart(curTurnBase);
            NextTurn();
        }
    }

    /// <summary>
    /// 战斗结束
    /// </summary>
    void CombatComplete(CombatResult result)
    {
        if (state == State.Combating)
        {
            state = State.CombatComplete;
            GameEvent.TurnBaseEvent.FireOnCombatComplete(curTurnBase, result);
            curTurnBase = null;
            nextTurnBase = null;
        }
    }

    /// <summary>
    /// 完成当前回合
    /// </summary>
    public void OnTurnComplete(TurnBase turnBase, TurnBaseRuningState result)
    {
        if (turnBase != curTurnBase)
        {
            Debug.LogError("结束的回合不是当前的回合");
            return;
        }

        switch (result)
        {
            case TurnBaseRuningState.NextTurn:
                NextTurn();
                break;
            case TurnBaseRuningState.Lost:
                CombatComplete(CombatResult.Lost);
                break;
            case TurnBaseRuningState.Win:
                CombatComplete(CombatResult.Win);
                break;
        }
    }

    /// <summary>
    /// 下一回合
    /// </summary>
    void NextTurn()
    {
        if (nextTurnBase == null)
        {
            curTurnBase = new TurnBase(1);
        }
        else
        {
            curTurnBase = nextTurnBase;
        }
        nextTurnBase = new TurnBase(curTurnBase.turnCounter);
        curTurnBase.Start();
    }
}

public enum CombatResult
{
    Abandon, //放弃
    Lost,     //失败
    Win,        //胜利
}