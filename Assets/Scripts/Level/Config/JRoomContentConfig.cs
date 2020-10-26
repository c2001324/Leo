using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class JRoomContentConfig
{

    public string parent;   //父类，要小心循环继承的问题
    public string name;
    public string script;
    public string param;
    public string model;

    //Key : 批次，小于等于0时，为预加载
    public Dictionary<int, JBatchContent> contents;

    /// <summary>
    /// 从父配置里继承数据
    /// </summary>
    /// <param name="parent"></param>
    public void InheritFromParent(JRoomContentConfig parent)
    {
        if (script == null || script == "")
        {
            script = parent.script;
        }
        if (param == null || param == "")
        {
            param = parent.param;
        }
        if (model == null || model == "")
        {
            model = parent.model;
        }
        if (contents == null && parent.contents != null)
        {
            contents = new Dictionary<int, JBatchContent>();
            var e = parent.contents.GetEnumerator();
            while (e.MoveNext())
            {
                contents.Add(e.Current.Key, new JBatchContent(e.Current.Value));
            }
        }
    }

    public JBatchContent GetBatchContent(int batch)
    {
        if (contents != null)
        {
            JBatchContent content = null;
            contents.TryGetValue(batch, out content);
            return content;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    ///  一个批次的内容
    /// </summary>
    public class JBatchContent
    {
        public JBatchContent()
        {

        }

        public JBatchContentElement[] elements;

        public JBatchContent(JBatchContent other)
        {
            if (other.elements != null)
            {
                elements = new JBatchContentElement[other.elements.Length];
                for (int i = 0; i < elements.Length; i++)
                {
                    elements[i] = new JBatchContentElement(other.elements[i]);
                }
            }
        }
    }

    public enum SpawnPointType
    {
        SpawnPoint, //指定的在spawnPoints里抽一个
        Average,     //随机平均                 无视SpawnPoint
        Near,          //在玩家身边生成
    }

    /// <summary>
    /// 指定出生点
    /// </summary>
    public class JBatchContentElement
    {
        public JBatchContentElement()
        {

        }

        public JBatchContentElement(JBatchContentElement other)
        {
            type = other.type;
            if (other.spawnPoints != null)
            {
                spawnPoints = new string[other.spawnPoints.Length];
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    spawnPoints[i] = other.spawnPoints[i];
                }
            }
            maxCount = other.maxCount;
            if (other.entityPools != null)
            {
                entityPools = new JEntityPool[other.entityPools.Length];
                for (int i =0; i < entityPools.Length; i++)
                {
                    entityPools[i] = new JEntityPool(other.entityPools[i]);
                }
            }
        }

        public SpawnPointType type;
        public string[] spawnPoints;
        public int maxCount = 0;
        public JEntityPool[] entityPools;

        public string GetSpawnPoint(System.Random random)
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                return null;
            }
            else
            {
                return spawnPoints[Untility.Tool.GetRandom(0, spawnPoints.Length, random)];
            }
        }

        public List<string> GetAllEntityNames(System.Random random)
        {
            List<string> allNames = new List<string>();
            if (entityPools != null)
            {
                foreach (JEntityPool pool in entityPools)
                {
                    int count = pool.count.GetValue(random);
                    for (int i = 0; i < count; i++)
                    {
                        allNames.Add(pool.entityName);
                    }
                }
                //随机
                Untility.Tool.RandSortList<string>(ref allNames, random);
                //删除多余的数量
                if (allNames.Count > maxCount && maxCount > 0)
                {
                    allNames.RemoveRange(0, allNames.Count - maxCount);
                }
            }
            return allNames;
        }
    }

    public class JEntityPool
    {
        public JEntityPool() { }

        public JEntityPool(JEntityPool other)
        {
            count = new JIntRange(other.count.min, other.count.max);
            entityName = other.entityName;
        }

        public JIntRange count = new JIntRange(1, 1);
        public string entityName;
    }

}

