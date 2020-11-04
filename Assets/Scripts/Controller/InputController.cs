using UnityEngine;
using UnityEditor;
using HexMap;

public class InputController : Controller
{

    CPropertyComponent m_PropertyComponent;

    protected override void OnBeginControll(IControllerAction controllerAction)
    {
        InputManager.instance.SetInputController(this);
        controllerAction.ShowMoveAndAttackRange();
        GameEvent.HexRoomEvent.onLeftClickCell.AddListener(OnLeftClickCell);
        GameEvent.HexRoomEvent.onRightClickCell.AddListener(OnRightClickCell);

        controllerAction.RegisterActionEvent(OnActionComplete);
        m_PropertyComponent = controllerAction.GetEntity().GetCComponent<CPropertyComponent>();
        m_NextStepAttackEntity = null;
    }

    protected override void OnStopControll(IControllerAction controllerAction)
    {
        InputManager.instance.RemoveController();
        controllerAction.HideMoveAndAttackRange();
        GameEvent.HexRoomEvent.onLeftClickCell.RemoveListener(OnLeftClickCell);
        GameEvent.HexRoomEvent.onRightClickCell.RemoveListener(OnRightClickCell);
        controllerAction.UnregisterActionEvent(OnActionComplete);
        m_PropertyComponent = null;
    }

    Entity m_NextStepAttackEntity;

    void OnLeftClickEntity(Entity entity)
    {

    }

    void OnRightClickEntity(Entity entity)
    {

    }

    void OnLeftClickCell(HexRoom room, HexCell cell)
    {

    }

    void OnRightClickCell(HexRoom room, HexCell cell)
    {
        if (cell.entity == null)
        {
            //判断是否可以移动到目标地点
            if (m_ControllerAction.IsCanMoveToDestination(cell))
            {
                m_ControllerAction.HideMoveAndAttackRange();
                m_ControllerAction.Move(cell, 0);
            }
        }
        else
        {
            //判断是否可以攻击目标
            if (m_ControllerAction.IsCanAttackTarget(cell.entity))
            {
                int targetDistance = GetEntity().centerCell.GetDistance(cell);
                int attackDistance = (int)m_PropertyComponent.attackRange;
                if (attackDistance >= targetDistance)
                {
                    m_ControllerAction.Attack(cell.entity);
                }
                else
                {
                    //先移动到附近
                    m_NextStepAttackEntity = cell.entity;
                    m_ControllerAction.Move(cell, attackDistance);
                }
            }
        }
    }

    void OnActionComplete(ControllerActionType type)
    {
        if (type == ControllerActionType.Move || type == ControllerActionType.Attack)
        {
            m_ControllerAction.ShowMoveAndAttackRange();
        }
        if (type == ControllerActionType.Move && m_NextStepAttackEntity != null)
        {
            m_ControllerAction.Attack(m_NextStepAttackEntity);
        }
    }

}