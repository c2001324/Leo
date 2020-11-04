using System;
using System.Collections.Generic;
using HexMap;
using UnityEngine;

public class CControllerComponent : CComponent, IComparable<CControllerComponent>, IControllerAction
{
    public CTeamComponent teamComponent { get; private set; }

    public TeamId teamId { get { return teamComponent.teamId; } }

    public Controller controller { get; private set; }

    MovementComponent m_Movement;

    CAttackComponent m_AttackComponent;

    CPropertyComponent m_PropertyComponent;

    protected override void OnInitializeComplete()
    {
        base.OnInitializeComplete();
        turnState = State.Complete;
        teamComponent = entity.GetCComponent<CTeamComponent>();
        m_Movement = entity.GetComponent<MovementComponent>();
        m_PropertyComponent = entity.GetCComponent<CPropertyComponent>();
        m_AttackComponent = entity.GetCComponent<CAttackComponent>();
        if (m_Movement != null)
        {
            m_Movement.onMoveComplete.AddListener(OnMoveComplete);
        }
        if (m_AttackComponent != null)
        {
            m_AttackComponent.onAttackComplete.AddListener(OnAttackComplete);
        }
    }



    #region 回合相关

    public enum State
    {
        Ready,        //等待自己的回合
        Playing,              //正在执行自己的回合
        Complete,           //回合完成
    }

    public State turnState { get; private set; }

    TurnBase m_TurnBase;

    /// <summary>
    /// 轮到当前的回合，激活回合
    /// </summary>
    public void ActiveTurnBase()
    {
        turnState = State.Ready;
    }

    /// <summary>
    /// 开始自己的回合
    /// </summary>
    public void TurnStart(TurnBase turnBase)
    {
        if (turnState == State.Ready)
        {
            m_TurnBase = turnBase;
            turnState = State.Playing;
            m_PropertyComponent.ResetMovePoint();
            onTurnStart.Invoke(this, m_TurnBase);
            GameEvent.TurnBaseEvent.FireOnEntityTurnStart(this);
            BeginControll();
        }
    }

    /// <summary>
    /// 自己回合结束
    /// </summary>
    public void TurnComplete()
    {
        if (turnState == State.Playing)
        {
            turnState = State.Complete;
            StopControll();
            onTurnComplete.Invoke(this, m_TurnBase);
            GameEvent.TurnBaseEvent.FireOnEntityTurnComplete(this);
            TurnBase tempTurn = m_TurnBase;
            m_TurnBase = null;
            tempTurn.OnEntityTurnComplete(this);  //这行代码一定需要放在函数的最后面，用于保证Entity回合结束后，清空所有的回合内容，再通知TurnBase
        }
    }

    protected override void OnBeginDestroy()
    {
        base.OnBeginDestroy();
        //死亡结束自己的回合
        TurnComplete();
    }
    #endregion

    #region 控制
    public void SetController(Controller controller)
    {
        this.controller = controller;
    }

    public void RemoveController()
    {
        controller = null;
    }

    void BeginControll()
    {
        controller.BeginControll(this);
    }

    void StopControll()
    {
        controller.StopControll(this);
        
    }
    #endregion


    public int CompareTo(CControllerComponent other)
    {
        return other.m_PropertyComponent.actionPriority.CompareTo(m_PropertyComponent.actionPriority);
    }

    #region 控制的动作接口，由Controll调用

    ControllerActionState m_ActionState = ControllerActionState.Ready;

    public ControllerActionState GetState()
    {
        return m_ActionState;
    }

    ControllerActionEvent m_ActionCompleteCallback = new ControllerActionEvent();

    public void RegisterActionEvent(Action<ControllerActionType> action)
    {
        m_ActionCompleteCallback.AddListener(action);
    }

    public void UnregisterActionEvent(Action<ControllerActionType> action)
    {
        m_ActionCompleteCallback.RemoveListener(action);
    }

    public Entity GetEntity()
    {
        return entity;
    }

    public void ShowMoveAndAttackRange()
    {
        if (m_Movement != null)
        {
            LevelManager.room.hexRoom.ShowMoveAndAttackRange(m_Movement.GetMoveRange(), m_AttackComponent.GetAttackRange());
        }
    }

    public void HideMoveAndAttackRange()
    {
        if (m_Movement != null)
        {
            LevelManager.room.hexRoom.HideMoveAndAttackRange();
        }
    }

    public bool Move(HexCell destination, int distance)
    {
        if (m_ActionState == ControllerActionState.Ready && m_Movement != null && m_Movement.MoveTo(destination, distance))
        {
            m_ActionState = ControllerActionState.DoingAction;
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnMoveComplete(HexCell cell)
    {
        m_ActionState = ControllerActionState.Ready;
        m_ActionCompleteCallback.Invoke(ControllerActionType.Move);
    }

    public bool Attack(Entity target)
    {
        if (m_ActionState == ControllerActionState.Ready && target != null && target != entity)
        {
            int distance = target.centerCell.GetDistance(entity.centerCell);
            int attackDistance = (int)m_PropertyComponent.attackRange;
            if (attackDistance >= distance)
            {
                //在攻击范围内，直接攻击
                m_AttackComponent.Attack(target);
                m_ActionState = ControllerActionState.DoingAction;
                return true;
            }
        }
        return false;
    }

    void OnAttackComplete()
    {
        m_ActionState = ControllerActionState.Ready;
        m_ActionCompleteCallback.Invoke(ControllerActionType.Attack);
    }

    public bool IsCanMoveToDestination(HexCell destination)
    {
        return m_Movement != null && m_Movement.IsCanMoveToDestination(destination);
    }

    public bool IsCanAttackTarget(Entity target)
    {
        int distance = target.centerCell.GetDistance(entity.centerCell);
        //攻击的距离
        int attackDistance = (int)m_PropertyComponent.attackRange;
        int moveDistance = (int)m_PropertyComponent.movePoint;
        return attackDistance + moveDistance >= distance;
    }
    #endregion



    public TurnEvent onTurnStart = new TurnEvent();
    public TurnEvent onTurnComplete = new TurnEvent();

    public TurnValueChangedEvent onTurnBaseValueChanged = new TurnValueChangedEvent();

    

    public class TurnEvent : CustomEvent<CControllerComponent, TurnBase> { }
    public class TurnValueChangedEvent : CustomEvent<CControllerComponent> { }

}
