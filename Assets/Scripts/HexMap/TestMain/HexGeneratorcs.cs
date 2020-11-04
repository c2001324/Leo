using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexMap
{
    public class HexGeneratorcs
    {

        public GridInfo result { get; private set; }

        public IEnumerator GenerateGrid(int width, int height, Transform parent, GameObject movablePrefab, GameObject[] heightPrefabes)
        {
            result = null;
            HexGridType hexGridType = width % 2 == 0 ? HexGridType.even_q : HexGridType.odd_q;
            List<HexCell> hexagons = new List<HexCell>();

            if (movablePrefab.GetComponent<HexCell>() == null)
            {
                Debug.LogError("Invalid hexagon prefab provided");
                yield break;
            }

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    GameObject hexagonObj;
                    if (UnityEngine.Random.Range(0f, 1f) < 0.8f)
                    {
                        hexagonObj = GameObject.Instantiate(movablePrefab) as GameObject;
                    }
                    else
                    {
                        hexagonObj = GameObject.Instantiate(heightPrefabes[UnityEngine.Random.Range(0, heightPrefabes.Length)]) as GameObject;
                    }
                    HexCell hexagon = hexagonObj.GetComponent<HexCell>();
                    var hexSize = hexagon.GetCellDimensions();

                    hexagon.transform.position = new Vector3(x * hexSize.x * 0.75f, 0f, (z * hexSize.z) + (x % 2 == 0 ? 0 : hexSize.z * 0.5f));
                    hexagon.offsetCoord = new Vector2(width - x - 1, height - z - 1);
                    hexagon.hexGridType = hexGridType;
                    hexagon.movementCost = 1;
                    hexagons.Add(hexagon);
                    hexagon.transform.parent = parent;
                    hexagon.gameObject.SetActive(false);
                    yield return null;
                }
            }
            var hexDimensions = movablePrefab.GetComponent<HexCell>().GetCellDimensions();
            var hexSide = hexDimensions.x / 2;

            result = new GridInfo();
            result.Cells = hexagons;

            result.Dimensions = new Vector3(hexSide * (width - 1) * 1.5f, hexDimensions.y * (height - 0.5f), hexDimensions.z);
            result.Center = result.Dimensions / 2;
        }
    }

    public class GridInfo
    {
        public Vector3 Dimensions { get; set; }
        public Vector3 Center { get; set; }
        public List<HexCell> Cells { get; set; }
    }
}
