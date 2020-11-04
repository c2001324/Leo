using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using HighlightingSystem;

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
    public class HexCell : MonoBehaviour, IGraphNode, IEquatable<HexCell>
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
        public List<HexCell> neighbours
        {
            get
            {
                if (m_Neighbours == null)
                {
                    m_Neighbours = new List<HexCell>(6);
                    foreach (var direction in m_CubeDirections)
                    {
                        var neighbour = room.cells.Find(c => c.offsetCoord == CubeToOffsetCoords(cubeCoord + direction));
                        if (neighbour == null) continue;
                        m_Neighbours.Add(neighbour);
                    }
                }
                return m_Neighbours;
            }
        }
        List<HexCell> m_Neighbours;

        /// <summary>
        /// 网格类型
        /// </summary>
        [HideInInspector]
        public HexGridType hexGridType;

        public virtual bool isCellMovableTo { get { return !isTaken; } }

        

        public HexRoom room { get; private set; }

        public Vector2 position2D { get { return new Vector2(transform.position.x, transform.position.z); } }

        public Vector3 entityStandPosition
        {
            get
            {
                return new Vector3(transform.position.x, m_MeshBounds.max.y * transform.localScale.y, transform.position.z);
            }
        }

        Bounds m_MeshBounds;

        Highlighter m_Highlighter;

        Renderer m_Renderer;

        #region 初始化
        public void Initialize(HexRoom room)
        {
            this.room = room;
            gameObject.SetActive(true);
            m_MeshBounds = gameObject.GetComponent<MeshFilter>().mesh.bounds;
            m_Highlighter = gameObject.GetComponent<Highlighter>();
            m_Renderer = gameObject.GetComponent<Renderer>();
            if (m_Highlighter == null)
            {
                m_Highlighter = gameObject.AddComponent<Highlighter>();
            }
            m_Highlighter.overlay = false;
        }

        public void CopyFields(HexCell newCell)
        {
            newCell.offsetCoord = offsetCoord;
            (newCell as HexCell).hexGridType = hexGridType;
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

        public int GetDistance(HexCell other)
        {
            var _other = other as HexCell;
            int distance = (int)(Mathf.Abs(cubeCoord.x - _other.cubeCoord.x) + Mathf.Abs(cubeCoord.y - _other.cubeCoord.y) + Mathf.Abs(cubeCoord.z - _other.cubeCoord.z)) / 2;
            return distance;
        }
        #endregion

        #region 获取各种Cell

        /// <summary>
        /// 获取一个环形
        /// </summary>
        public List<HexCell> GetRings(int radius)
        {
            if (radius == 0)
            {
                return new List<HexCell>(1){ this};
            }

            List<HexCell> results = new List<HexCell>();
            Vector3 cube = cubeCoord + CubeCoordDirection(4) * radius;
            for (int direction =0; direction < 6; direction++)
            {
                for (int j = 0; j < radius; j++)
                {
                    HexCell cell = room.GetCell(cube);
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
        public List<HexCell> GetSpiralRings(int radius)
        {
            List<HexCell> results = new List<HexCell>();
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
        public List<HexCell> GetLine(HexCell end)
        {
            List<HexCell> results = new List<HexCell>();
            float distance = GetDistance(end);
            for (int i = 0; i <= (int)distance; i++)
            {
                Vector3 tempCube = CubeCoordRound(CubeCoordLerp(this.cubeCoord, end.cubeCoord, 1f / distance * i));
                HexCell temp = room.GetCell(tempCube);
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
        public virtual bool IsCellTraversable(HexCell originCell)
        {
            if (!isCellMovableTo)
            {
                return false;
            }
            else
            {
                return Mathf.Abs(((HexCell)originCell).height - (this.height)) <= 1f;
            }
        }

        public Vector3 GetCellDimensions()
        {
            return new Vector3(size * 2f, 1f, Mathf.Sqrt(3) * size);
        }

        #region 事件

        void OnMouseEnter()
        {
            if (!InputManager.instance.IsOverlapUI() && room != null)
            {
                room.OnSelectCell(this);
                if (entity != null)
                {
                    entity.OnSelectCell(this);
                }
            }
        }

        void OnMouseExit()
        {
            if (!InputManager.instance.IsOverlapUI() && room != null)
            {
                room.OnUnselectCell(this);
                if (entity != null)
                {
                    entity.OnUnselectCell(this);
                }
            }
        }

        void OnMouseOver()
        {
            if (!InputManager.instance.IsOverlapUI() && room != null)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    room.OnLeftClickCell(this);
                    if (entity != null)
                    {
                        entity.OnLeftClickCell(this);
                    }
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    room.OnRightClickCell(this);
                    if (entity != null)
                    {
                        entity.OnRightClickCell(this);
                    }
                }
            }
        }

        void OnMouseDown()
        {
            
        }

        #endregion

        public Entity entity { get; private set; }

        public void SetEntity(Entity entity)
        {
            if (this.entity != null)
            {
                Debug.LogError("已经被Entity占用了");
            }
            else
            {
                this.entity = entity;
            }
        }

        public void RemoveEntity()
        {
            entity = null;
        }

        void SetColor(Color color)
        {
            foreach (Material mat in m_Renderer.materials)
            {
                mat.color = color;
            }
        }

        public void MarkMoveRange()
        {
            SetColor(Color.yellow);
        }

        public void MarkAttackRange()
        {
            SetColor(Color.red);
        }

        public void UnMark()
        {
            SetColor(Color.white);
        }

        public void Select()
        {
            m_Highlighter.constant = true;
            m_Highlighter.constantColor = Color.green;
        }

        public void Unselect()
        {
            m_Highlighter.constant = false;
        }

        public int GetDistance(IGraphNode other)
        {
            return GetDistance(other as HexCell);
        }

        public virtual bool Equals(HexCell other)
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
            if (!(other is HexCell))
                return false;

            return Equals(other as HexCell);
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
        public HexagonsWithCenter(HexCell center, List<HexCell> allCells)
        {
            this.center = center;
            this.allCells = allCells;
        }

        public HexagonsWithCenter(HexCell center)
        {
            this.center = center;
            allCells = new List<HexCell>() { center };
        }

        public HexCell center;
        public List<HexCell> allCells;
    }
}