using System.Collections;
using UnityEngine;

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
                StartCoroutine(GeneratorMap());   
            }
        }

        IEnumerator GeneratorMap()
        {
            HexGeneratorcs generator = new HexGeneratorcs();
            GameObject obj = new GameObject();
            obj.transform.parent = transform;
            yield return generator.GenerateGrid(15, 15, obj.transform, movablePrefab, heightPrefab);
            HexRoom map = new HexRoom(generator.result.Cells);
        }

    }
}
