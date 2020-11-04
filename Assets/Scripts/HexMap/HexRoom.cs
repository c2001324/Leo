using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexMap
{
    public class HexRoom
    {
        public HexRoom(List<HexCell> cells)
        {
            this.cells = new List<HexCell>(cells.Count);
            foreach (HexCell cell in cells)
            {
                cell.Initialize(this);
                this.cells.Add(cell);
            }
        }

        #region 管理cells
        public List<HexCell> cells { get; private set; }

        public HexCell GetCell(Vector3 cubeCoord)
        {
            foreach (HexCell cell in cells)
            {
                if (cell.EqualsCubeCoord(cubeCoord))
                {
                    return cell;
                }
            }
            return null;
        }

        public HexCell GetCell(Vector2 offsetCoord)
        {
            foreach (HexCell cell in cells)
            {
                if (cell.EqualsOffsetCoord(offsetCoord))
                {
                    return cell;
                }
            }
            return null;
        }

        #endregion

        #region cell事件，由HexCell回调

        public void OnLeftClickCell(HexCell selectCell)
        {
            GameEvent.HexRoomEvent.FireOnLeftClickCell(this, selectCell);
        }

        public void OnRightClickCell(HexCell selectCell)
        {
            GameEvent.HexRoomEvent.FireOnRightClickCell(this, selectCell);
        }

        public void OnSelectCell(HexCell selectCell)
        {
            UnselectAllCell();
            selectCell.Select();
            GameEvent.HexRoomEvent.FireOnSelectCell(this, selectCell);
        }

        public void OnUnselectCell(HexCell selectCell)
        {
            UnselectAllCell();
            GameEvent.HexRoomEvent.FireOnUnselectCell(this, selectCell);
        }

        void UnselectAllCell()
        {
            foreach (HexCell cell in cells)
            {
                cell.Unselect();
            }
        }
        #endregion

        #region entity被选中的事件
        public void OnSelectEntity(Entity entity)
        {
            OnSelectCell(entity.centerCell);
        }

        public void OnUnselectEntity(Entity entity)
        {
            OnUnselectCell(entity.centerCell);
        }

        public void OnLeftClickEntity(Entity entity)
        {
            OnLeftClickCell(entity.centerCell);
        }

        public void OnRightClickEntity(Entity entity)
        {
            OnRightClickCell(entity.centerCell);
        }
        #endregion

        #region 对外接口
        public void ShowMoveAndAttackRange(List<HexCell> moveDestinations, List<HexCell> attackRange)
        {
            var except = cells.Except<HexCell>(attackRange);
            var onlyAttackRange = attackRange.Except<HexCell>(moveDestinations);
            foreach (HexCell cell in except)
            {
                cell.UnMark();
            }
            foreach (HexCell cell in moveDestinations)
            {
                cell.MarkMoveRange();
            }
            foreach (HexCell cell in onlyAttackRange)
            {
                cell.MarkAttackRange();
            }
        }

        public void HideMoveAndAttackRange()
        {
            foreach (HexCell cell in cells)
            {
                cell.UnMark();
            }
        }
        #endregion

        #region 获取cell
        /// <summary>
        /// 在全地图内获取没有被占用的cell
        /// </summary>
        /// <param name="size">占用大小</param>
        /// <param name="centerCell">占用的中心点</param>
        /// <param name="allCells">所有占用的格子</param>
        /// <returns></returns>
        public HexagonsWithCenter FindClearCells(int size)
        {
            if (size <= 1)
            {
                HexCell centerCell = FindClearCell();
                if (centerCell != null)
                {
                    return new HexagonsWithCenter(centerCell, new List<HexCell>() { centerCell });
                }
                else
                {
                    return null;
                }
            }

            int ringRadius = size - 1;
            foreach (int index in GetRandomCellIndexCache())
            {
                HexagonsWithCenter result = GetSpiralRingsWithSameHeight(cells[index], ringRadius);
                if (result != null)
                {
                    return null;
                }
            }
            return null;
        }


        /// <summary>
        /// 在全地图内获取内获取一个没有被占用的Cell
        /// </summary>
        /// <returns></returns>
        public HexCell FindClearCell()
        {
            foreach (int index in GetRandomCellIndexCache())
            {
                if (!cells[index].isTaken)
                {
                    return cells[index];
                }
            }
            return null;
        }

        /// <summary>
        /// 在指定的范围内获取一个Cell
        /// </summary>
        /// <param name="range">0是代表只选取center</param>
        ///<param name="random">是否全部随机，如果不是就从近到远获取</param>
        ///<param name="exceptRange">排除的范围，0表示排除center</param>
        public HexCell FindClearCellInRange(HexCell center, int range = -1, int exceptRange = -1, bool random = false)
        {
            if (range == 0)
            {
                if (center.isTaken)
                {
                    return center;
                }
                else
                {
                    return null;
                }
            }

            if (random)
            {
                //从全范围内随机
                List<HexCell> tempList = null;
                if (exceptRange >= 0)
                {
                    //被除队的范围
                    List<HexCell> exceptTempList = center.GetSpiralRings(exceptRange);
                    if (range < 0)
                    {
                        tempList = cells.Except(exceptTempList).ToList<HexCell>();
                    }
                    else
                    {
                        tempList = center.GetSpiralRings(range).Except(exceptTempList).ToList<HexCell>();
                    }
                }
                else
                {
                    if (range < 0)
                    {
                        tempList = new List<HexCell>(cells);
                    }
                    else
                    {
                        tempList = center.GetSpiralRings(range);
                    }
                }

                Untility.Tool.RandSortList<HexCell>(ref tempList);
                foreach (HexCell tempCell in tempList)
                {
                    if (!tempCell.isTaken)
                    {
                        return tempCell;
                    }
                }
                return null;
            }
            else
            {
                //从近到远
                int i = exceptRange >= 0 ? exceptRange + 1: 0;
                int maxRange = range > 0 ? range : int.MaxValue;
                for (; i <= maxRange; i++)
                {
                    List<HexCell> tempList = center.GetRings(i);
                    if (tempList.Count < 0)
                    {
                        break;
                    }
                    Untility.Tool.RandSortList<HexCell>(ref tempList);
                    foreach (HexCell tempCell in tempList)
                    {
                        if (!tempCell.isTaken)
                        {
                            return tempCell;
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 在指定的范围内获取一个Cell
        /// </summary>
        /// <param name="range">0是代表只选取center，小于0是代表全范围，只是计算center这点，size大于一时可能会超出Range的范围</param>
        ///<param name="random">是否全部随机，如果不是就从近到远获取</param>
        ///<param name="exceptRange">排除的范围，0表示排除center，只是排除center这点，并不会影响到全范围的cell</param>
        public HexagonsWithCenter FindClearCellsInRange(int size, HexCell center, int range = -1, int exceptRange = -1, bool random = false)
        {
            if (size <= 1)
            {
                HexCell cell = FindClearCellInRange(center, range, exceptRange, random);
                if (cell != null)
                {
                    return new HexagonsWithCenter(cell);
                }
                else
                {
                    return null;
                }
            }

            if (range == 0)
            {
                if (center.isTaken)
                {
                    return GetSpiralRingsWithSameHeight(center, size - 1);
                }
                else
                {
                    return null;
                }
            }

            if (random)
            {
                //从全范围内随机
                List<HexCell> tempList = null;
                if (exceptRange >= 0)
                {
                    //被除队的范围
                    List<HexCell> exceptTempList = center.GetSpiralRings(exceptRange);
                    if (range < 0)
                    {
                        tempList = cells.Except(exceptTempList).ToList<HexCell>();
                    }
                    else
                    {
                        tempList = center.GetSpiralRings(range).Except(exceptTempList).ToList<HexCell>();
                    }
                    
                }
                else
                {
                    if (range < 0)
                    {
                        tempList = new List<HexCell>(cells);
                    }
                    else
                    {
                        tempList = center.GetSpiralRings(range);
                    }
                }

                Untility.Tool.RandSortList<HexCell>(ref tempList);
                foreach (HexCell tempCell in tempList)
                {
                    if (!tempCell.isTaken)
                    {
                        HexagonsWithCenter result = GetSpiralRingsWithSameHeight(tempCell, size - 1);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
                return null;
            }
            else
            {
                //从近到远
                int i = exceptRange >= 0 ? exceptRange + 1 : 0;
                int maxRange = range > 0 ? range : int.MaxValue;
                for (; i <= maxRange; i++)
                {
                    List<HexCell> tempList = center.GetRings(i);
                    if (tempList.Count <= 0)
                    {
                        break;
                    }
                    Untility.Tool.RandSortList<HexCell>(ref tempList);
                    foreach (HexCell tempCell in tempList)
                    {
                        if (!tempCell.isTaken)
                        {
                            HexagonsWithCenter result = GetSpiralRingsWithSameHeight(tempCell, size - 1);
                            if (result != null)
                            {
                                return result;
                            }
                        }
                    }
                }
                return null;
            }
        }

        #region 内部使用的函数
        /// <summary>
        /// 获取点的一个螺旋环内所有等高并没有被占用的cell
        /// </summary>
        HexagonsWithCenter GetSpiralRingsWithSameHeight(HexCell centerCell, int ringRadius)
        {
            if (centerCell.isTaken)
            {
                //中心点被占用
                return null;
            }

            List<HexCell> allResults = centerCell.GetSpiralRings(ringRadius);
            if (allResults.Count != HexCell.GetSpiralRingCount(ringRadius))
            {
                //数量不足
                return null;
            }

            bool result = true;
            int height = -1;
            foreach (var tempCell in allResults)
            {
                if (tempCell.isTaken)
                {
                    result = false;
                    break;
                }
                if (height < 0)
                {
                    height = tempCell.height;
                }
                if (height != tempCell.height)
                {
                    //高度不统一
                    result = false;
                    break;
                }
            }

            if (result)
            {
                return new HexagonsWithCenter(centerCell, allResults);
            }
            else
            {
                return null;
            }
        }


        List<int> m_RandomCellIndexCache;

        /// <summary>
        /// 获取一级随机的cell 索引
        /// </summary>
        /// <returns></returns>
        List<int> GetRandomCellIndexCache()
        {
            if (m_RandomCellIndexCache == null)
            {
                m_RandomCellIndexCache = Untility.Tool.CreateRandomList(0, cells.Count - 1);
            }
            else
            {
                Untility.Tool.RandSortList<int>(ref m_RandomCellIndexCache);
            }
            return m_RandomCellIndexCache;
        }
        #endregion

        #endregion

        #region 寻路
        private static DijkstraPathfinding m_Pathfinder = new DijkstraPathfinding();
        private static IPathfinding m_FallbackPathfinder = new AStarPathfinding();

        public class HexCellPaths : Dictionary<HexCell, List<HexCell>>
        {
            public List<HexCell> FindPath(HexCell destination)
            {
                List<HexCell> list;
                TryGetValue(destination, out list);
                return list;
            }

            public List<HexCell> GetDestinations()
            {
                return Keys.ToList<HexCell>();
            }
        }

        /// <summary>
        /// 查找可以到达的cell
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="originCell"></param>
        /// <param name="movementPoint"></param>
        /// <returns></returns>
        public HexCellPaths GetAvailableDestinations(HexCell originCell, float movementPoint)
        {
            HexCellPaths result = new HexCellPaths();
            var paths = CachePaths(cells, originCell);
            foreach (var key in paths.Keys)
            {
                if (!key.isCellMovableTo)
                {
                    continue;
                }
                var path = paths[key];
                var pathCost = path.Sum(c => c.movementCost);
                if (pathCost <= movementPoint)
                {
                    result.Add(key, path);
                }
            }
            return result;
        }

        Dictionary<HexCell, List<HexCell>> CachePaths(List<HexCell> cells, HexCell originCell)
        {
            var edges = GetGraphEdges(cells, originCell);
            var paths = m_Pathfinder.findAllPaths(edges, originCell);
            return paths;
        }

        public static void SortPathByDistance(List<HexCell> path, HexCell destination)
        {
            path.Sort(
            delegate (HexCell a, HexCell b)
            {
                int aDistance = destination.GetDistance(a);
                int bDistance = destination.GetDistance(b);
                return bDistance.CompareTo(aDistance);
            });
        }

        public List<HexCell> FindPath(HexCellPaths paths, HexCell origin, HexCell destination)
        {
            if (paths != null)
            {
                return paths.FindPath(destination);
            }
            else
            {
                return m_FallbackPathfinder.FindPath(GetGraphEdges(cells, origin), origin, destination);
            }
        }
        /// <summary>
        /// Method returns graph representation of cell grid for pathfinding.
        /// </summary>
        protected virtual Dictionary<HexCell, Dictionary<HexCell, float>> GetGraphEdges(List<HexCell> cells, HexCell orgin)
        {
            Dictionary<HexCell, Dictionary<HexCell, float>> ret = new Dictionary<HexCell, Dictionary<HexCell, float>>();
            foreach (var cell in cells)
            {
                if (cell.isCellMovableTo || cell.Equals(orgin))
                {
                    ret[cell] = new Dictionary<HexCell, float>();
                    foreach (var neighbour in cell.neighbours.FindAll(delegate (HexCell targetCell) { return targetCell.IsCellTraversable(cell); }))
                    {
                        ret[cell][neighbour] = neighbour.movementCost;
                    }
                }
            }
            return ret;
        }
        #endregion

        #region 消息

//         readonly public HexEvent onSelectCell = new HexEvent();
//         readonly public HexEvent onUnSelectCell = new HexEvent();
//         readonly public HexEvent onLeftClickCell = new HexEvent();
//         readonly public HexEvent onRightClickCell = new HexEvent();
// 
        public class HexEvent : CustomEvent<HexCell> { }
        #endregion

    }
}
