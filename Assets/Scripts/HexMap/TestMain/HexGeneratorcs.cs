using System.Collections.Generic;
using UnityEngine;

namespace HexMap
{
    public class HexGeneratorcs
    {
        public GridInfo GenerateGrid(int width, int height, Transform parent, GameObject movablePrefab, GameObject[] heightPrefabes)
        {
            HexGridType hexGridType = width % 2 == 0 ? HexGridType.even_q : HexGridType.odd_q;
            List<Hexagon> hexagons = new List<Hexagon>();

            if (movablePrefab.GetComponent<Hexagon>() == null)
            {
                Debug.LogError("Invalid hexagon prefab provided");
                return null;
            }

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    GameObject hexagon;
                    if (UnityEngine.Random.Range(0f, 1f) < 0.8f)
                    {
                        hexagon = GameObject.Instantiate(movablePrefab) as GameObject;
                    }
                    else
                    {
                        hexagon = GameObject.Instantiate(heightPrefabes[UnityEngine.Random.Range(0, heightPrefabes.Length)]) as GameObject;
                    }
                    var hexSize = hexagon.GetComponent<Hexagon>().GetCellDimensions();

                    hexagon.transform.position = new Vector3(x * hexSize.x * 0.75f, 0f, (z * hexSize.z) + (x % 2 == 0 ? 0 : hexSize.z * 0.5f));
                    hexagon.GetComponent<Hexagon>().offsetCoord = new Vector2(width - x - 1, height - z - 1);
                    hexagon.GetComponent<Hexagon>().hexGridType = hexGridType;
                    hexagon.GetComponent<Hexagon>().movementCost = 1;
                    hexagons.Add(hexagon.GetComponent<Hexagon>());

                    hexagon.transform.parent = parent;
                }
            }
            var hexDimensions = movablePrefab.GetComponent<Hexagon>().GetCellDimensions();
            var hexSide = hexDimensions.x / 2;

            GridInfo gridInfo = new GridInfo();
            gridInfo.Cells = hexagons;

            gridInfo.Dimensions = new Vector3(hexSide * (width - 1) * 1.5f, hexDimensions.y * (height - 0.5f), hexDimensions.z);
            gridInfo.Center = gridInfo.Dimensions / 2;

            return gridInfo;
        }
    }

    public class GridInfo
    {
        public Vector3 Dimensions { get; set; }
        public Vector3 Center { get; set; }
        public List<Hexagon> Cells { get; set; }
    }
}
