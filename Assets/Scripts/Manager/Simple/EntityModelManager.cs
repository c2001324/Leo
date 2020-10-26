using System.Collections.Generic;
using UnityEngine;
using Untility;

/// <summary>
/// 模型管理器
/// 加载管理所有的ModelBase
/// </summary>
public class EntityModelManager : Singleton<EntityModelManager>, ISimpleInitManager
{
    public void Initialize()
    {
        
    }

    Dictionary<string, EntityModel> m_Cache = new Dictionary<string, EntityModel>();

    

    public EntityModel GetEntityModel(string path)
    {
        if (path == null || path == "")
        {
            return null;
        }
        path = path.ToLower();
        EntityModel m = null;
        if (m_Cache.TryGetValue(path, out m))
        {
            return m;
        }
        else
        {
            m = LoadEntityModel(path);
            if (m != null)
            {
                m_Cache.Add(path, m);
                return m;
            }   
            else
            {
                Debug.LogError("找不到模型 " + path);
                return null;
            }
        }
    }

    EntityModel LoadEntityModel(string path)
    {
        GameObject obj = Resources.Load<GameObject>(path);
        if (obj != null)
        {
            return obj.GetComponent<EntityModel>();
        }
        
        return null;
    }
}
