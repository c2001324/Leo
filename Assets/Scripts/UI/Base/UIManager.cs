using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Untility;
using UnityEngine.EventSystems;


/// <summary>
/// UI管理器，管理关卡内所有的窗口。
/// 改造需求：需要把界面划分为“战斗窗口”和“战斗外窗口”，用于内存优化
/// </summary>
public class UIManager : Singleton<UIManager>, ISyncInitManager
{
    public UIRoot uiRoot { get; private set; }
    

    public bool IsTest = false;
    private Dictionary<string, UIWindow> m_UIWindowList = new Dictionary<string, UIWindow>();
    private Dictionary<string, JWindowConfig> m_UIWindowConfigs = new Dictionary<string, JWindowConfig>();
    private Dictionary<int, GameObject> m_LevelObjs = new Dictionary<int, GameObject>();

    public Transform GetUILevel(int level)
    {
        if (m_LevelObjs.ContainsKey(level))
        {
            return m_LevelObjs[level].transform;
        }
        else
        {
            return null;
        }
    }

    public UIManager()
    {
        
    }

    public IEnumerable<ManagerProgress> Initialize()
    {
        LoadConfig();
        uiRoot = UIRoot.Create();
        GameObject temSys = GameObject.Find("EventSystem");
        if (temSys == null)
        {
            temSys = new GameObject("EventSystem");
            temSys.AddComponent<EventSystem>();
            temSys.AddComponent<StandaloneInputModule>();
        }
        yield return new ManagerProgress(1f, "");
    }



    /// <summary>
    /// 创建窗口
    /// </summary>
    /// <param name="uiName">类名</param>
    /// <returns></returns>
    public UIWindow CreateUI(string uiName)
    {
        return _createUI(uiName);
    }

    /// <summary>
    /// 获取selfObject上附属的UIWindow
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="selfObj">游戏对象</param>
    /// <param name="parent">父窗口</param>
    /// <returns></returns>
    public T GetComponentToGameObject<T>(GameObject selfObj, UIWindow parent = null) where T : UIWindow
    {
        string className = typeof(T).Name;
        T wnd = _createUI(className, parent, selfObj) as T;
        //创建的子窗口，默认可见
        if (wnd != null)

        {
            wnd.Open(true);
        }
        return wnd;
    }

    /// <summary>
    /// 获取窗口
    /// </summary>
    /// <param name="uiName">窗口类名</param>
    /// <returns></returns>
    public UIWindow GetUI(string uiName, bool bCreate)
    {
        UIWindow wnd = null;
        if (m_UIWindowList.TryGetValue(uiName, out wnd))
        {
            return wnd;
        }
        else if (bCreate)
        {
            wnd = _createUI(uiName);
            return wnd;
        }
        else
        {
            return null;
        }
    }

    public T GetUI<T>(bool bCreate = true) where T : UIWindow
    {
        UIWindow wnd = GetUI(typeof(T).Name.ToString(), bCreate);
        if (wnd != null)
        {
            return (T)wnd;
        }
        return null;
    }

    public bool IsShow<T>() where T : UIWindow
    {
        UIWindow wnd = GetUI<T>(false);
        return wnd != null && wnd.isShow;
    }

    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    /// <param name="useAnimation">是否使用渐变动画（待后期添加更多的动画效果时，再进行改造）</param>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="param3"></param>
    /// <returns></returns>
    public T OpenUI<T>(bool useAnimation, object param1 = null, object param2 = null, object param3 = null) where T : UIWindow
    {
        UIWindow wnd = OpenUI(typeof(T).Name, useAnimation, param1, param2, param3);
        if (wnd != null)
        {
            return (T)wnd;
        }
        return null;
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <typeparam name="T">窗口类型</typeparam>
    /// <param name="useAnimation">是否使用动画</param>
    public void CloseUI<T>() where T : UIWindow
    {
        UIWindow wnd = GetUI<T>(false);
        if (wnd != null && wnd.isShow)
        {
            wnd.Close();
        }
    }

    public UIWindow OpenUI(string uiName, bool useAniatiom, object param1 = null, object param2 = null, object param3 = null)
    {
        UIWindow wnd = GetUI(uiName, true);
        if (wnd == null)
        {
            wnd = _createUI(uiName);
            if (wnd == null)
            {
                return null;
            }
        }
        wnd.Open(useAniatiom, param1, param2, param3);
        return wnd;
    }


    public void DestroyUI<T>() where T : UIWindow
    {
        string name = typeof(T).Name.ToString();
        if (m_UIWindowList.ContainsKey(name))
        {
            UIWindow wnd = GetUI<T>();
            wnd.Destroy();
            m_UIWindowList.Remove(name);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    private void LoadConfig()
    {
        foreach(var config in LoadJsonObject.CreateObjectFromResource<JWindowConfig[]>("Config/UIConfig"))
        {
            if (m_UIWindowConfigs.ContainsKey(config.name))
            {
                Debug.LogError("已存在相同项 " + config.name);
            }
            else
            {
                m_UIWindowConfigs.Add(config.name, config);
            }
        }
    }

    /// <summary>
    /// 创建ui
    /// </summary>
    /// <param name="uiName">ui类的名称</param>
    /// <param name="parent">父窗口，当窗口为空时，UIWindowConfig.level生效，窗口对象直接挂在对应的level上</param>
    /// <param name="targetObject">ui的依附游戏对象</param>
    /// <returns></returns>
    private UIWindow _createUI(string uiName, UIWindow parent = null, GameObject selfObject = null)
    {
        UIWindow wnd;
        JWindowConfig structData = _getStructData(uiName);
        if (structData != null)
        {
            wnd = _createUIByStructData(structData, selfObject);
            if (parent != null)
            {
                return wnd;
            }
            else if (wnd != null)
            {
                //设置窗口层
                if (_setUILeve(structData.layer, wnd))
                {
                    m_UIWindowList[uiName] = wnd;
                    return wnd;
                }
                else
                {
                    Debug.LogError("设置UI level 失败");
                    return null;
                }
            }
        }
        else
        {
            Debug.LogError("没有找到" + uiName + "窗口");
        }
        return null;
    }


    private JWindowConfig _getStructData(string name)
    {
        JWindowConfig config = null;
        m_UIWindowConfigs.TryGetValue(name, out config);
        return config;
    }


    private Type _getTypeByString(string name)
    {
        string className = name;
        if (this.GetType().Namespace != null)
        {
            className = this.GetType().Namespace + "." + className;
        }

        return Type.GetType(className);
    }

    /// <summary>
    /// 创建ui类
    /// </summary>
    /// <param name="config"></param>
    /// <param name="targetObject">ui类属于的对像，如果为空，即创建类的gameObject</param>
    /// <returns></returns>
    private UIWindow _createUIByStructData(JWindowConfig config, GameObject targetObject)
    {
        GameObject objTem = targetObject;
        if (targetObject == null)
        {
            objTem = Resources.Load<GameObject>(config.prefab);
        }

        if (objTem != null)
        {
            Type type = _getTypeByString(config.name);
            if (type != null)
            {
                GameObject obj = targetObject;
                if (obj == null)
                {
                    obj = GameObject.Instantiate(objTem, objTem.transform.position, objTem.transform.rotation) as GameObject;

                    Resources.UnloadUnusedAssets();

                    obj.name = config.name;
                }

                UIWindow wnd = obj.GetComponent(type) as UIWindow;

                if (wnd == null)
                {
                    wnd = obj.AddComponent(type) as UIWindow;
                    if (wnd == null)
                    {
                        Debug.LogError(config.name + "没有组件" + config.name);
                    }
                }
                if (wnd != null)
                {
                    wnd.Initialize(config);
                    wnd.SetShow(false);
                    return wnd;
                }
                return null;
            }
            else
            {
                Debug.LogError(config.name + "没有组件" + config.name);
            }

        }
        else
        {
            Debug.Log("找不到Prefab " + config.name);
        }
        return null;
    }

    /// <summary>
    /// 获取UI层级的游戏对象
    /// </summary>
    /// <param name="layer">层级</param>
    /// <returns></returns>
    private GameObject _getUILevel(int layer)
    {
        if (m_LevelObjs.ContainsKey(layer))
        {
            return m_LevelObjs[layer];
        }
        else
        {
            GameObject layerObj = new GameObject(layer.ToString());
            layerObj.transform.SetParent(uiRoot.transform, false);

            RectTransform rect = layerObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2();
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2();
            rect.sizeDelta = new Vector2();
            rect.localScale = new Vector3(1, 1, 1);

            m_LevelObjs.Add(layer, layerObj);

            if (m_LevelObjs.Count == 0)
            {
                Debug.LogError("first : level = " + layer);
                layerObj.transform.SetSiblingIndex(0);
            }
            else
            {
                List<int> keyList = new List<int>(m_LevelObjs.Keys);
                keyList.Sort();

                int index = 0;
                for (int i = 0; i < keyList.Count; i++)
                {
                    if (keyList[i] == layer)
                    {
                        break;
                    }
                    index++;
                }
                layerObj.transform.SetSiblingIndex(m_LevelObjs.Count - index);
            }


            return layerObj;
        }
    }

    private bool _setUILeve(int level, UIWindow wnd)
    {
        GameObject obj = _getUILevel(level);
        if (obj == null)
        {
            return false;
        }
        else
        {
            wnd.gameObject.transform.SetParent(obj.transform, false);
            return true;
        }
    }

    static public void DestoryUI(UIWindow wnd)
    {
        if (instance.m_UIWindowList.ContainsKey(wnd.name))
        {
            instance.m_UIWindowList.Remove(wnd.name);
        }
        GameObject.DestroyImmediate(wnd.gameObject);
    }

    public void FireAfterOpenWndEvent(UIWindow wnd)
    {
        onAfterOpenWnd.Invoke(wnd);
    }

    public void FireAfterCloseWndEvent(UIWindow wnd)
    {
        onAfterCloseWnd.Invoke(wnd);
    }

    public void FireBeforeOpenWndEvent(UIWindow wnd)
    {
        onbeforeOpenWnd.Invoke(wnd);
    }

    public void FireBeforeCloseWndEvent(UIWindow wnd)
    {
        onbeforeCloseWnd.Invoke(wnd);
    }

    readonly public OnAfterOpenWndEvent onAfterOpenWnd = new OnAfterOpenWndEvent();
    readonly public OnAfterCloseWndEvent onAfterCloseWnd = new OnAfterCloseWndEvent();

    readonly public OnBeforeOpenWndEvent onbeforeOpenWnd = new OnBeforeOpenWndEvent();
    readonly public OnBeforeCloseWndEvent onbeforeCloseWnd = new OnBeforeCloseWndEvent();

    public class OnAfterOpenWndEvent : CustomEvent<UIWindow> { }
    public class OnAfterCloseWndEvent : CustomEvent<UIWindow> { }

    public class OnBeforeOpenWndEvent : CustomEvent<UIWindow> { }
    public class OnBeforeCloseWndEvent : CustomEvent<UIWindow> { }

}

public class JWindowConfig
{
    public string name;
    public string prefab;
    public int layer;
    public bool hasMessage;
}

public enum WindowAnimatiomType
{
    None,       //没有动画
    Pop,            //弹出动画
    Falling,        //下落动画
}