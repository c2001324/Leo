using UnityEngine;
using HexMap;
using System.Collections.Generic;

/// <summary>
/// 移动组件
/// </summary>
public class MovementComponent : EntityComponentBase
{
    CAnimatorComponent m_AnimatorComponent;

    CPropertyComponent m_PropertyComponent;

    protected override void OnInitializeComplete()
    {
        base.OnInitializeComplete();
        m_AnimatorComponent = entity.GetCComponent<CAnimatorComponent>();
        m_PropertyComponent = entity.GetCComponent<CPropertyComponent>();
    }

    public bool isMoving { get { return m_MovingData != null; } }

    public bool IsCanMoveToDestination(HexCell destination)
    {
        return GetMoveRange().Contains(destination);
    }

    /// <summary>
    /// 更新移动范围
    /// </summary>
    public List<HexCell> GetMoveRange()
    {
        HexRoom.HexCellPaths movePath = LevelManager.room.hexRoom.GetAvailableDestinations(entity.centerCell, m_PropertyComponent.movePoint);
        return movePath.GetDestinations();
    }

    private void Update()
    {
        if (m_AnimatorComponent != null)
        {
            if (isMoving)
            {
                m_AnimatorComponent.Move();
            }
            else
            {
                m_AnimatorComponent.Idle();
            }
        }
    }

    #region 移动

    public bool MoveTo(HexCell destination, int distance = 0)
    {
        if (isMoving)
        {
            Debug.LogError("当前正在移动，不可以发出新的移动指令");
            return false;
        }

        List<HexCell> path = null;

        if (distance == 0)
        {
            HexRoom.HexCellPaths allPath = LevelManager.room.hexRoom.GetAvailableDestinations(entity.centerCell, m_PropertyComponent.movePoint);
            path = LevelManager.room.hexRoom.FindPath(allPath, entity.centerCell, destination);
            HexRoom.SortPathByDistance(path, destination);
        }
        else
        {
            //和目标保持一定的距离
            int targetDistance = entity.centerCell.GetDistance(destination);
            int moveDistance = targetDistance - distance;
            moveDistance = moveDistance < 0 ? 0 : moveDistance;
            if (m_PropertyComponent.movePoint < moveDistance)
            {
                Debug.LogError("移动力不足 " + moveDistance);
                return false;
            }

            HexRoom.HexCellPaths allPath = LevelManager.room.hexRoom.GetAvailableDestinations(entity.centerCell, targetDistance);
            path = LevelManager.room.hexRoom.FindPath(allPath, entity.centerCell, destination);
            HexRoom.SortPathByDistance(path, destination);
            for (int i = 0; i < distance; i++)
            {
                path.RemoveAt(path.Count - 1);
            }
        }

        m_MovingData = new MovingData(path, path[path.Count - 1], entity, m_PropertyComponent);
        if (m_MovingData.BeginMove())
        {
            onBeginMove.Invoke(destination);
            InvokeRepeating("OnMoving", 0f, Config.system.baseTimeDelta);
            return true;
        }
        else
        {
            m_MovingData = null;
            return false;
        }
    }

    MovingData m_MovingData;

    class MovingData
    {
        public MovingData(List<HexCell> movingPath, HexCell destination, Entity entity, CPropertyComponent propertyComponent)
        {
            m_PropertyComponent = propertyComponent;
            m_Entity = entity;
            m_TargetCellIndex = -1;
            m_MovingPath = new List<HexCell>(movingPath);
        }

        public HexCell curCell { get { return m_MovingPath[m_TargetCellIndex]; } }


        CPropertyComponent m_PropertyComponent;
        Entity m_Entity;
        int m_TargetCellIndex;
        List<HexCell> m_MovingPath;
        Vector3 m_Forward;

        public bool BeginMove()
        {
            return NextCell();
        }

        public bool OnMoving()
        {
            if (GetDistance() <= 0.1f)
            {
                m_Entity.SetCell(new HexagonsWithCenter(curCell));
                return NextCell();
            }
            else
            {
                m_Entity.position += m_Forward * Config.system.moveSpeed * Config.system.baseTimeDelta;
                return true;
            }
        }

        bool NextCell()
        {
            if (m_TargetCellIndex + 1 >= m_MovingPath.Count)
            {
                return false;
            }
            else
            {
                if (m_PropertyComponent.CostMovePoint(1f))
                {
                    m_TargetCellIndex++;
                    m_Forward = m_MovingPath[m_TargetCellIndex].entityStandPosition - m_Entity.position;
                    m_Forward.Normalize();
                    m_Entity.forward = new Vector3(m_Forward.x, 0f, m_Forward.z);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        float GetDistance()
        {
            return Vector3.Distance(m_MovingPath[m_TargetCellIndex].entityStandPosition, m_Entity.position);
        }
    }


    void OnMoving()
    {
        if (!m_MovingData.OnMoving())
        {
            CancelInvoke("OnMoving");
            onMoveComplete.Invoke(m_MovingData.curCell);
            m_MovingData = null;
        }
    }
    #endregion



    public MoveEvent onBeginMove = new MoveEvent();
    public MoveEvent onMoveComplete = new MoveEvent();

    public class MoveEvent : CustomEvent<HexCell> { }
}

