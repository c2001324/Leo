using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Untility;

/// <summary>
/// 动画曲线管理器
/// </summary>
public class AnimationCurveManager : Singleton<AnimationCurveManager>, IDonotInitManager
{

    Dictionary<string, AnimationCurve> m_Pool = new Dictionary<string, AnimationCurve>();

    public AnimationCurve GetAnimationCurve(string name, bool addToPool = false)
    {
        string trueName = name.ToLower();
        AnimationCurve ac = null;
        if (m_Pool.TryGetValue(trueName, out ac))
        {
            return ac;
        }
        else
        {
            JAnimationCurveData data = LoadAnimationCurveData(trueName);
            if (data != null)
            {
                ac = new AnimationCurve(data.keyDatas);
                if (addToPool)
                {
                    m_Pool.Add(trueName, ac);
                }
            }
            return ac;
        }
    }


    JAnimationCurveData LoadAnimationCurveData(string name)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, "AnimationCurve");
        AssetBundle ab = AssetBundle.LoadFromFile(fullPath);
        TextAsset data = ab.LoadAsset<TextAsset>(name.ToLower());
        ab.Unload(false);
        if (data == null)
        {
            Debug.LogError("找不到动画曲线：" + name);
            return null;
        }
        else
        {
            return JsonConvert.DeserializeObject<JAnimationCurveData>(data.text);
        }
    }

}

public class JAnimationCurveData
{
    public JAnimationCurveData()
    {

    }

    public JAnimationCurveData(string name, AnimationCurve ac)
    {
        this.name = name;
        keyDatas = ac.keys;
    }

    public string name { get; set; }
    public Keyframe[] keyDatas;
}
