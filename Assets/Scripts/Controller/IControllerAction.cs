using UnityEngine;
using UnityEditor;
using HexMap;
using System;

public enum ControllerActionState
{
    Ready,
    DoingAction,
}

public enum ControllerActionType
{
    Move,
    Attack
}

public class ControllerActionEvent : CustomEvent<ControllerActionType> { }


public interface IControllerAction
{
    ControllerActionState GetState();

    void RegisterActionEvent(Action<ControllerActionType> action);

    void UnregisterActionEvent(Action<ControllerActionType> action);

    #region 移动
    bool Move(HexCell destination, int distance);

    void ShowMoveAndAttackRange();

    void HideMoveAndAttackRange();

    bool IsCanMoveToDestination(HexCell destination);
    #endregion

    bool Attack(Entity target);

    

    bool IsCanAttackTarget(Entity target);

    Entity GetEntity();
}