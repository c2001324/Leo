using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RoomModelEntity : MonoBehaviour
{
    public string entityName;


    public EntityData GetEntityData()
    {
        return new EntityData()
        {
            position = transform.position,
            rotation = transform.rotation,
            name = entityName
        };
    }

    public class EntityData
    {
        public Vector3 position;
        public Quaternion rotation;
        public string name;
    }

#if UNITY_EDITOR
    EntityModel m_Model;
    string m_OldEntityName = "";


    private void Update()
    {
        InitManager();
        if (m_InitManager && m_OldEntityName != entityName)
        {
            m_OldEntityName = entityName;
            JEntityConfig config = EntityManager.instance.GetEntityConfig(entityName);

            foreach (EntityModel model in GetComponentsInChildren<EntityModel>())
            {
                DestroyImmediate(model.gameObject);
            }

            if (config != null)
            {
                EntityModel model = EntityModelManager.instance.GetEntityModel(config.entityModel);
                model = GameObject.Instantiate<GameObject>(model.gameObject).GetComponent<EntityModel>();
                model.transform.SetParent(transform, false);
                gameObject.name = entityName;
            }
        }
    }


    static bool m_InitManager = false;
    static void InitManager()
    {
        if (!m_InitManager)
        {
            EntityManager.instance.Initialize();
            EntityModelManager.instance.Initialize();
            m_InitManager = true;
        }
    }
    

#endif
}
