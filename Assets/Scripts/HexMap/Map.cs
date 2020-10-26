using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexMap
{
    public class Map
    {
        public Map(List<Hexagon> cells)
        {
            this.cells = new List<Hexagon>(cells.Count);
            foreach (Hexagon cell in cells)
            {
                cell.Initialize(this);
                cell.onClicked += OnClickCell;
                cell.onEnter += OnEnterCell;
                cell.onExit += OnExitCell;
                this.cells.Add(cell);
            }
        }

        #region 管理cells
        public List<Hexagon> cells { get; private set; }

        public Hexagon GetCell(Vector3 cubeCoord)
        {
            foreach (Hexagon cell in cells)
            {
                if (cell.EqualsCubeCoord(cubeCoord))
                {
                    return cell;
                }
            }
            return null;
        }

        public Hexagon GetCell(Vector2 offsetCoord)
        {
            foreach (Hexagon cell in cells)
            {
                if (cell.EqualsOffsetCoord(offsetCoord))
                {
                    return cell;
                }
            }
            return null;
        }

        #endregion

        #region cell事件

        void OnClickCell(Hexagon selectCell, EventArgs param)
        {
            ShowPath(selectCell);
        }

        void OnEnterCell(object obj, EventArgs param)
        {

        }

        void OnExitCell(object obj, EventArgs param)
        {

        }

        #endregion


        HashSet<Hexagon> m_PathsInRange;

        

        

        void TestFindClearCell(Hexagon selectCell)
        {
            HexagonsWithCenter result = FindClearCells(2);

            if (result != null)
            {
                var cellsNotInRange = cells.Except(result.allCells);
                foreach (var cell in cellsNotInRange)
                {
                    if (cell != selectCell)
                    {
                        cell.UnMark();
                    }
                }
                foreach (var cell in result.allCells)
                {
                    if (cell != selectCell)
                    {
                        cell.MarkAsReachable();
                    }
                }
            }
        }

        void ShowPath(Hexagon selectCell)
        {
            m_PathsInRange = GetAvailableDestinations(cells, selectCell, 6);
            var cellsNotInRange = cells.Except(m_PathsInRange);

            foreach (var cell in cellsNotInRange)
            {
                if (cell != selectCell)
                {
                    cell.UnMark();
                }
            }
            foreach (var cell in m_PathsInRange)
            {
                if (cell != selectCell)
                {
                    cell.MarkAsReachable();
                }
            }
        }

        void ShowSpawnCell(Hexagon selectCell, int size)
        {
            m_PathsInRange = GetAvailableDestinations(cells, selectCell, 3);
            var cellsNotInRange = cells.Except(m_PathsInRange);

            foreach (var cell in cellsNotInRange)
            {
                if (cell != selectCell)
                {
                    cell.UnMark();
                }
            }
            foreach (var cell in m_PathsInRange)
            {
                if (cell != selectCell)
                {
                    cell.MarkAsReachable();
                }
            }
        }

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
                Hexagon centerCell = FindClearCell();
                if (centerCell != null)
                {
                    return new HexagonsWithCenter(centerCell, new List<Hexagon>() { centerCell });
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
        public Hexagon FindClearCell()
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
        public Hexagon FindClearCellInRange(Hexagon center, int range = -1, int exceptRange = -1, bool random = false)
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
                List<Hexagon> tempList = null;
                if (exceptRange >= 0)
                {
                    //被除队的范围
                    List<Hexagon> exceptTempList = center.GetSpiralRings(exceptRange);
                    if (range < 0)
                    {
                        tempList = cells.Except(exceptTempList).ToList<Hexagon>();
                    }
                    else
                    {
                        tempList = center.GetSpiralRings(range).Except(exceptTempList).ToList<Hexagon>();
                    }
                }
                else
                {
                    if (range < 0)
                    {
                        tempList = new List<Hexagon>(cells);
                    }
                    else
                    {
                        tempList = center.GetSpiralRings(range);
                    }
                }

                Untility.Tool.RandSortList<Hexagon>(ref tempList);
                foreach (Hexagon tempCell in tempList)
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
                    List<Hexagon> tempList = center.GetRings(i);
                    if (tempList.Count < 0)
                    {
                        break;
                    }
                    Untility.Tool.RandSortList<Hexagon>(ref tempList);
                    foreach (Hexagon tempCell in tempList)
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
        public HexagonsWithCenter FindClearCellsInRange(int size, Hexagon center, int range = -1, int exceptRange = -1, bool random = false)
        {
            if (size <= 1)
            {
                Hexagon cell = FindClearCellInRange(center, range, exceptRange, random);
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
                List<Hexagon> tempList = null;
                if (exceptRange >= 0)
                {
                    //被除队的范围
                    List<Hexagon> exceptTempList = center.GetSpiralRings(exceptRange);
                    if (range < 0)
                    {
                        tempList = cells.Except(exceptTempList).ToList<Hexagon>();
                    }
                    else
                    {
                        tempList = center.GetSpiralRings(range).Except(exceptTempList).ToList<Hexagon>();
                    }
                    
                }
                else
                {
                    if (range < 0)
                    {
                        tempList = new List<Hexagon>(cells);
                    }
                    else
                    {
                        tempList = center.GetSpiralRings(range);
                    }
                }

                Untility.Tool.RandSortList<Hexagon>(ref tempList);
                foreach (Hexagon tempCell in tempList)
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
                    List<Hexagon> tempList = center.GetRings(i);
                    if (tempList.Count <= 0)
                    {
                        break;
                    }
                    Untility.Tool.RandSortList<Hexagon>(ref tempList);
                    foreach (Hexagon tempCell in tempList)
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
        HexagonsWithCenter GetSpiralRingsWithSameHeight(Hexagon centerCell, int ringRadius)
        {
            if (centerCell.isTaken)
            {
                //中心点被占用
                return null;
            }

            List<Hexagon> allResults = centerCell.GetSpiralRings(ringRadius);
            if (allResults.Count != Hexagon.GetSpiralRingCount(ringRadius))
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

        Dictionary<Hexagon, List<Hexagon>> m_CachedPaths = new Dictionary<Hexagon, List<Hexagon>>();

        /// <summary>
        /// 查找可以到达的cell
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="originCell"></param>
        /// <param name="movementPoint"></param>
        /// <returns></returns>
        public HashSet<Hexagon> GetAvailableDestinations(List<Hexagon> cells, Hexagon originCell, float movementPoint)
        {
            m_CachedPaths.Clear();
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
                    m_CachedPaths.Add(key, path);
                }
            }
            return new HashSet<Hexagon>(m_CachedPaths.Keys);
        }

        Dictionary<Hexagon, List<Hexagon>> CachePaths(List<Hexagon> cells, Hexagon originCell)
        {
            var edges = GetGraphEdges(cells, originCell);
            var paths = m_Pathfinder.findAllPaths(edges, originCell);
            return paths;
        }

        public List<Hexagon> FindPath(List<Hexagon> cells, Hexagon origin, Hexagon destination)
        {
            if (m_CachedPaths != null && m_CachedPaths.ContainsKey(destination))
            {
                return m_CachedPaths[destination];
            }
            else
            {
                return m_FallbackPathfinder.FindPath(GetGraphEdges(cells, origin), origin, destination);
            }
        }
        /// <summary>
        /// Method returns graph representation of cell grid for pathfinding.
        /// </summary>
        protected virtual Dictionary<Hexagon, Dictionary<Hexagon, float>> GetGraphEdges(List<Hexagon> cells, Hexagon orgin)
        {
            Dictionary<Hexagon, Dictionary<Hexagon, float>> ret = new Dictionary<Hexagon, Dictionary<Hexagon, float>>();
            foreach (var cell in cells)
            {
                if (cell.isCellMovableTo || cell.Equals(orgin))
                {
                    ret[cell] = new Dictionary<Hexagon, float>();
                    foreach (var neighbour in cell.neighbours.FindAll(delegate (Hexagon targetCell) { return targetCell.IsCellTraversable(cell); }))
                    {
                        ret[cell][neighbour] = neighbour.movementCost;
                    }
                }
            }
            return ret;
        }
        #endregion

    }
}
