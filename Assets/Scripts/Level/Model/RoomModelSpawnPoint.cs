using UnityEngine;

public class RoomModelSpawnPoint : MonoBehaviour
{
    public class SpawnPointData
    {
        public float size;
        public string name;
        public Vector3 position;
    }

    public SpawnPointData GetSpawnPointData()
    {
        return new SpawnPointData()
        {
            size = size,
            name = gameObject.name,
            position = transform.position
        };
    }

    public float size = 2;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, size);
    }
#endif
}
