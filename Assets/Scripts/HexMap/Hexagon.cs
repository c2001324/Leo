using System.Collections.Generic;
using UnityEngine;
using System;

namespace HexMap
{
    public enum HexGridType
    {
        even_q,
        odd_q,
        even_r,
        odd_r
    };

    /// <summary>
    /// Implementation of hexagonal cell.
    /// </summary>
    public class Hexagon : MonoBehaviour, IGraphNode, IEquatable<Hexagon>
    {

        #region 可配置的属性

        public float size = 0f;

        public int height = 0;

        #endregion

        /// <summary>
        /// 是否有单位占用了
        /// </summary>
        public bool isTaken { get; private set; }

        public float movementCost = 1f;

        /// <summary>
        /// 邻居
        /// </summary>
        public List<Hexagon> neighbours
        {
            get
            {
                if (m_Neighbours == null)
                {
                    m_Neighbours = new List<Hexagon>(6);
                    foreach (var direction in m_CubeDirections)
                    {
                        var neighbour = map.cells.Find(c => c.offsetCoord == CubeToOffsetCoords(cubeCoord + direction));
                        if (neighbour == null) continue;
                        m_Neighbours.Add(neighbour);
                    }
                }
                return m_Neighbours;
            }
        }
        List<Hexagon> m_Neighbours;

        /// <summary>
        /// 网格类型
        /// </summary>
        [HideInInspector]
        public HexGridType hexGridType;

        public virtual bool isCellMovableTo { get { return !isTaken; } }

        

        private Renderer m_Renderer;

        public Map map { get; private set; }

        #region 初始化
        public void Initialize(Map map)
        {
            this.map = map;
            m_Renderer = GetComponent<Renderer>();
            SetColor(m_Renderer, Color.white);
        }

        public void CopyFields(Hexagon newCell)
        {
            newCell.offsetCoord = offsetCoord;
            (newCell as Hexagon).hexGridType = hexGridType;
        }

        #endregion

        #region 坐标和坐标转换
        /// <summary>
        /// 在网格中的坐标
        /// </summary>
        public Vector2 offsetCoord { get; set; }

        /// <summary>
        /// Cube坐标
        /// </summary>
        protected Vector3 cubeCoord
        {
            get
            {
                Vector3 ret = new Vector3();
                switch (hexGridType)
                {
                    case HexGridType.odd_q:
                        {
                            ret.x = offsetCoord.x;
                            ret.z = offsetCoord.y - (offsetCoord.x + (Mathf.Abs(offsetCoord.x) % 2)) / 2;
                            ret.y = -ret.x - ret.z;
                            break;
                        }
                    case HexGridType.even_q:
                        {
                            ret.x = offsetCoord.x;
                            ret.z = offsetCoord.y - (offsetCoord.x - (Mathf.Abs(offsetCoord.x) % 2)) / 2;
                            ret.y = -ret.x - ret.z;
                            break;
                        }
                }
                return ret;
            }
        }


        protected Vector2 CubeToOffsetCoords(Vector3 cubeCoords)
        {
            Vector2 ret = new Vector2();

            switch (hexGridType)
            {
                case HexGridType.odd_q:
                    {
                        ret.x = cubeCoords.x;
                        ret.y = cubeCoords.z + (cubeCoords.x + (Mathf.Abs(cubeCoords.x) % 2)) / 2;
                        break;
                    }
                case HexGridType.even_q:
                    {
                        ret.x = cubeCoords.x;
                        ret.y = cubeCoords.z + (cubeCoords.x - (Mathf.Abs(cubeCoords.x) % 2)) / 2;
                        break;
                    }
            }
            return ret;
        }
        #endregion

        #region 数学运算
        protected static readonly Vector3[] m_CubeDirections =  {
        new Vector3(+1, -1, 0), new Vector3(+1, 0, -1), new Vector3(0, +1, -1),
        new Vector3(-1, +1, 0), new Vector3(-1, 0, +1), new Vector3(0, -1, +1)};


        Vector3 CubeCoordDirection(int direction)
        {
            return m_CubeDirections[direction];
        }


        Vector3 CubeCoordHeighbor(Vector3 cubeCoord, int direction)
        {
            return cubeCoord + CubeCoordDirection(direction);
        }

        float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }



        public Vector3 CubeCoordLerp(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(Lerp(a.x, b.x, t),
                Lerp(a.y, b.y, t),
                Lerp(a.z, b.z, t));
        }

        public  Vector3 CubeCoordRound(Vector3 cube)
        {
            var rx = Mathf.Round(cube.x);
            var ry = Mathf.Round(cube.y);
            var rz = Mathf.Round(cube.z);

            var x_diff = Mathf.Abs(rx - cube.x);
            var y_diff = Mathf.Abs(ry - cube.y);
            var z_diff = Mathf.Abs(rz - cube.z);

            if (x_diff > y_diff && x_diff > z_diff)
            {
                rx = -ry - rz;
            }
            else if (y_diff > z_diff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }
            return new Vector3(rx, ry, rz);
        }

        public int GetDistance(Hexagon other)
        {
            var _other = other as Hexagon;
            int distance = (int)(Mathf.Abs(cubeCoord.x - _other.cubeCoord.x) + Mathf.Abs(cubeCoord.y - _other.cubeCoord.y) + Mathf.Abs(cubeCoord.z - _other.cubeCoord.z)) / 2;
            return distance;
        }
        #endregion

        #region 获取各种Cell

        /// <summary>
        /// 获取一个环形
        /// </summary>
        public List<Hexagon> GetRings(int radius)
        {
            if (radius == 0)
            {
                return new List<Hexagon>(1){ this};
            }

            List<Hexagon> results = new List<Hexagon>();
            Vector3 cube = cubeCoord + CubeCoordDirection(4) * radius;
            for (int direction =0; direction < 6; direction++)
            {
                for (int j = 0; j < radius; j++)
                {
                    Hexagon cell = map.GetCell(cube);
                    if (cell != null)
                    {
                        results.Add(cell);
                    }
                    cube = CubeCoordHeighbor(cube, direction);
                }
            }
            return results;
        }

        /// <summary>
        /// 获取整个环形的cell数量
        /// </summary>
        /// <param name="raduis"></param>
        /// <returns></returns>
        static public int GetRingCount(int raduis)
        {
            return raduis * 6;
        }

        /// <summary>
        /// 获取螺旋环形
        /// </summary>
        public List<Hexagon> GetSpiralRings(int radius)
        {
            List<Hexagon> results = new List<Hexagon>();
            results.Add(this);
            for (int i = 1; i <= radius; i++)
            {
                results.AddRange(GetRings(i));
            }
            return results;
        }

        /// <summary>
        /// 获取螺旋环形的数量
        /// </summary>
        static public int GetSpiralRingCount(int radius)
        {
            int count = 1;
            for (int i = 1; i <= radius; i++)
            {
                count += GetRingCount(i);
            }
            return count;
        }

        /// <summary>
        /// 获取到目标点的一条直接上所有的Cell
        /// </summary>
        /// <param name="map"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Hexagon> GetLine(Hexagon end)
        {
            List<Hexagon> results = new List<Hexagon>();
            float distance = GetDistance(end);
            for (int i = 0; i <= (int)distance; i++)
            {
                Vector3 tempCube = CubeCoordRound(CubeCoordLerp(this.cubeCoord, end.cubeCoord, 1f / distance * i));
                Hexagon temp = map.GetCell(tempCube);
                if (temp != null)
                {
                    results.Add(temp);
                }
            }
            return results;
        }
        #endregion
        
        /// <summary>
        /// 是否可以从某个cell移动到这个cell
        /// </summary>
        public virtual bool IsCellTraversable(Hexagon originCell)
        {
            if (!isCellMovableTo)
            {
                return false;
            }
            else
            {
                return Mathf.Abs(((Hexagon)originCell).height - (this.height)) <= 1f;
            }
        }

        public Vector3 GetCellDimensions()
        {
            return new Vector3(size * 2f, 1f, Mathf.Sqrt(3) * size);
        }

        private void SetColor(Renderer renderer, Color color)
        {
            renderer.material.color = color;
        }

        #region 事件

        protected virtual void OnMouseEnter()
        {
            if (onEnter != null)
            {
                onEnter.Invoke(this, new EventArgs());
            }
            SetColor(m_Renderer, Color.blue);
        }

        protected virtual void OnMouseExit()
        {
            if (onExit != null)
            {
                onExit.Invoke(this, new EventArgs());
            }
            SetColor(m_Renderer, Color.white);
        }

        protected virtual void OnMouseDown()
        {
            if (onClicked != null)
            {
                onClicked.Invoke(this, new EventArgs());
            }
        }

        public delegate void HexEventHandler(Hexagon cell, EventArgs e);
        public event HexEventHandler onClicked;
        public event HexEventHandler onEnter;
        public event HexEventHandler onExit;     
        #endregion

        public void MarkAsReachable()
        {
            SetColor(m_Renderer, Color.yellow);
        }
        
        public void MarkAsPath()
        {
            SetColor(m_Renderer, Color.yellow);
        }
        
        public void MarkAsHighlighted()
        {
            
        }
        
        public void UnMark()
        {
            SetColor(m_Renderer, Color.white);
        }

        public int GetDistance(IGraphNode other)
        {
            return GetDistance(other as Hexagon);
        }

        public virtual bool Equals(Hexagon other)
        {
            return (offsetCoord.x == other.offsetCoord.x && offsetCoord.y == other.offsetCoord.y);
        }

        public bool EqualsOffsetCoord(Vector2 coord)
        {
            return offsetCoord.x == coord.x && offsetCoord.y == coord.y;
        }

        public bool EqualsCubeCoord(Vector3 coord)
        {
            return cubeCoord.x == coord.x && cubeCoord.y == coord.y && cubeCoord.z == coord.z;
        }

        public override bool Equals(object other)
        {
            if (!(other is Hexagon))
                return false;

            return Equals(other as Hexagon);
        }

        public override int GetHashCode()
        {
            int hash = 23;

            hash = (hash * 37) + (int)offsetCoord.x;
            hash = (hash * 37) + (int)offsetCoord.y;
            return hash;
        }
    }

    public class HexagonsWithCenter
    {
        public HexagonsWithCenter(Hexagon center, List<Hexagon> allCells)
        {
            this.center = center;
            this.allCells = allCells;
        }

        public HexagonsWithCenter(Hexagon center)
        {
            this.center = center;
            allCells = new List<Hexagon>() { center };
        }

        public Hexagon center;
        public List<Hexagon> allCells;
    }
}