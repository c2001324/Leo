using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class RoomModel : MonoBehaviour
{
    #region 编辑模式
#if UNITY_EDITOR
    public static RoomModel Create(string name)
    {
        GameObject obj = new GameObject();
        obj.name = name;
        RoomModel roomModel = obj.AddComponent<RoomModel>();
        GameObject beginPosition = new GameObject("beginPosition");
        beginPosition.AddComponent<RoomModelBeginPoint>();
        beginPosition.transform.parent = obj.transform;

        GameObject ground = new GameObject("Ground");
        ground.AddComponent<RoomModelGround>();
        ground.transform.parent = obj.transform;
        ground.layer = LayerMask.NameToLayer(Layers.ground);
        return roomModel;
    }

    public void CreatStaff()
    {
        GameObject obj = new GameObject("Staff");
        obj.AddComponent<RoomModelBeginPoint>();
        obj.transform.parent = transform;
    }

    public void CreateWall()
    {
        GameObject obj = new GameObject();
        obj.transform.parent = transform;
        obj.name = "Wall";
        obj.layer = LayerMask.NameToLayer(Layers.wall);
    }

    public bool IsChild(GameObject target)
    {
        if (gameObject == target)
        {
            return true;
        }
        else
        {
            Transform temp = target.transform;
            while (temp.parent != null)
            {
                if (temp.parent == gameObject.transform)
                {
                    return true;
                }
                else
                {
                    temp = temp.parent;
                }
            }
            return false;
        }
    }

    public bool isInstance
    {
        get
        {
            string path = AssetDatabase.GetAssetPath(GetInstanceID());
            return path == null || path == "";
        }
    }

    public string prefabPath
    {
        get
        {
            return AssetDatabase.GetAssetPath(GetInstanceID());
        }
    }

    public bool Check()
    {
        RoomModelBeginPoint[] points = GetComponentsInChildren<RoomModelBeginPoint>();
        if (points.Length != 1)
        {
            Debug.LogError("每个房间必须有一个 beginPoint");
            return false;
        }

        RoomModelGround[] grounds = GetComponentsInChildren<RoomModelGround>();
        if (grounds.Length != 1)
        {
            Debug.LogError("每个房间必须只有一个 ground");
            return false;
        }
        return true;
    }

#endif
    #endregion


    public Vector3 beginPoint { get; private set; } 

    public Rect rect { get; private set; }

    public RoomModelSpawnPoint.SpawnPointData[] spawnPoints { get; private set; }

    public RoomModelEntity.EntityData[] entities { get; private set; }

    public string key { get; private set; }

    public void Initialize(string key)
    {
        this.key = key;
        beginPoint = GetComponentInChildren<RoomModelBeginPoint>().transform.position;
        BoxCollider c = GetComponentInChildren<RoomModelGround>().GetComponent<BoxCollider>();
        Vector2 position = new Vector2(c.size.x * -0.5f + c.center.x, c.size.z * -0.5f + c.center.z);
        rect = new Rect(position.x, position.y, c.size.x, c.size.z);
        InitEntity();
        InitSpawnPoint();
    }

    void InitEntity()
    {
        RoomModelEntity[] targets = GetComponentsInChildren<RoomModelEntity>();
        entities = new RoomModelEntity.EntityData[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            entities[i] = targets[i].GetEntityData();
            Destroy(targets[i].gameObject);
        }
    }

    void InitSpawnPoint()
    {
        RoomModelSpawnPoint[] targets = GetComponentsInChildren<RoomModelSpawnPoint>();
        spawnPoints = new RoomModelSpawnPoint.SpawnPointData[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            spawnPoints[i] = targets[i].GetSpawnPointData();
            Destroy(targets[i].gameObject);
        }
    }

    public RoomModelSpawnPoint.SpawnPointData GetSpawnPoint(string name, System.Random random)
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            if (name != null && name != "")
            {
                foreach (RoomModelSpawnPoint.SpawnPointData data in spawnPoints)
                {
                    if (data.name == name)
                    {
                        return data;
                    }
                }
            }
            //随机返回一个
            int index = Untility.Tool.GetRandom(0, spawnPoints.Length, random);
            return spawnPoints[index];
        }
        else
        {
            return null;
        }
    }
}
