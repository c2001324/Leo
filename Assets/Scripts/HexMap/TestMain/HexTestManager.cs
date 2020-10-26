using UnityEngine;
using UnityEditor;

namespace HexMap
{
    public class HexTestManager : MonoBehaviour
    {
        public GameObject movablePrefab;
        public GameObject []heightPrefab;

        GameObject LoadHexPrefab()
        {
            GameObject obj = GameObject.Instantiate<GameObject>(movablePrefab);
            return obj;
        }

        bool m_PressKey = false;

        private void Update()
        {
            if (!m_PressKey && Input.GetKeyUp(KeyCode.Space))
            {
                HexGeneratorcs generator = new HexGeneratorcs();
                GridInfo info = generator.GenerateGrid(10, 10, transform, movablePrefab, heightPrefab);
                Map map = new Map(info.Cells);
            }
        }
    }
}
