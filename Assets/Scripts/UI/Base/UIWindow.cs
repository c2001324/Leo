using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Game.UI里的所有类，是由UIManager来管理、创建，
/// 并在uimanager.csv表里维护所有的窗口类
/// 并且每一个窗口类，都有一个与之对应的Prefab，在Resources/Prefab/UI路径下
/// </summary>

[RequireComponent(typeof(Image))]
public class UIWindow : MonoBehaviour
{
    //不规则窗口事件的支持
    PolygonCollider2D m_Collider;
    
    protected bool m_bIsCreated = false;

    public bool isShow

    {
        get
        {
            return gameObject.activeSelf;
        }
    }

   
    //透明
    public bool transparent
    {
        set
        {
            if (value)
            {
                backGround.color = new Color(1, 1, 1, 0);
            }
            else
            {
                backGround.color = new Color(1, 1, 1, 1);
            }
        }
        get
        {
            return backGround.color.a != 0;
        }
    }


    public Image backGround { get { return m_Panel; } }
    Image m_Panel;

    public new string name { get { return m_Config.name; } }


    JWindowConfig m_Config;


    CanvasGroup m_CanvasGroup;

    //窗口是否正在打开本窗口
    bool m_bIsOpening = false;

    public bool isClosing { get { return m_bIsClosing; } }
    bool m_bIsClosing = false;


    /// <summary>
    /// 为确保正确的初始化顺序，Initialize 在UIWindow创建后手动调用
    /// </summary>
    public void Initialize(JWindowConfig config)
    {
        m_Panel = gameObject.GetComponent<Image>();
        m_Collider = GetComponent<PolygonCollider2D>();
        m_bIsCreated = true;
        m_Config = config;
        m_CanvasGroup = GetComponent<CanvasGroup>();
        if (m_CanvasGroup == null)
        {
            m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
            m_CanvasGroup.alpha = 0;
        }
        
        m_Panel.raycastTarget = false;
        if (!_initialize())
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    protected virtual bool _initialize()
    {
        try
        {
            afterCreate();
            m_CanvasGroup.alpha = 0;
        }
        catch (Exception e)
        {
            Debug.LogError(gameObject.name + " afterCreate error : " + e.Message + "\n" + e.StackTrace);
            return false;
        }
        return true;
    }

   
    bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
    {

        int count = polyPoints.Length;
        if (count < 3)
        {
            return false;
        }

        bool result = false;
        p = p / UIManager.instance.uiRoot.compressRotio;

        for (int i = 0, j = count - 1; i < count; i++)
        {
            Vector2 p1 = transform.TransformDirection(polyPoints[i]);
            Vector2 p2 = transform.TransformDirection(polyPoints[j]);

            if (p1.y < p.y && p2.y >= p.y || p2.y < p.y && p1.y >= p.y)
            {
                if (p1.x + (p.y - p1.y) / (p2.y - p1.y) * (p2.x - p1.x) < p.x)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////公有函数//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////重载//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    protected virtual void afterCreate()
    {

    }

    protected virtual bool beforeOpen(object param1 = null, object param2 = null, object param3 = null)
    {
        return true;
    }

    protected virtual void afterOpen()
    {

    }

    protected virtual void beforeClose()
    {

    }

    protected virtual void afterClose()
    {

    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////封装引擎的函数//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////内部函数//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    

    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="param3"></param>
    public void Open(bool useAnimation, object param1 = null, object param2 = null, object param3 = null)
    {
        if (beforeOpen(param1, param2, param3))
        {
            UIManager.instance.FireBeforeOpenWndEvent(this);
            gameObject.SetActive(true);
            m_bIsOpening = true;
            CancelInvoke();
            DoOpenWindow();
        }
    }

    void DoOpenWindow()
    {
        m_CanvasGroup.alpha = 1f;
        DoAfterOpen();
    }

    void DoAfterOpen()
    {
        afterOpen();
        UIManager.instance.FireAfterOpenWndEvent(this);
    }

    //关闭窗口
    public virtual void Close()
    {
        if (isShow)
        {
            m_bIsOpening = false;
            m_bIsClosing = true;
            UIManager.instance.FireBeforeCloseWndEvent(this);
            beforeClose();
            m_CanvasGroup.alpha = 0;
            DoClose();
        }
    }

    /// <summary>
    /// 执行关闭窗口
    /// </summary>
    void DoClose()
    {
        if (!m_bIsOpening && m_bIsClosing)
        {
            m_bIsClosing = false;
            gameObject.SetActive(false);
            afterClose();
            UIManager.instance.FireAfterCloseWndEvent(this);
        }

    }

    /// <summary>
    /// 使用应该函数时，不会调用beforeOpen和beforeClose
    /// </summary>
    /// <param name="show"></param>
    public void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }

    public void Destroy()
    {
        Close();
        OnWindowDestroy();
        GameObject.DestroyImmediate(gameObject);
    }

    protected virtual void OnWindowDestroy()
    {

    }

    /// <summary>
    /// 计算字符串的长度
    /// </summary>
    /// <param name="font"></param>
    /// <param name="str"></param>
    /// <returns></returns>
    public static float GetStringWidth(Text text)
    {
        float w = text.cachedTextGenerator.GetPreferredWidth(text.text, text.GetGenerationSettings(text.rectTransform.rect.size));
        return text.preferredWidth;
    }

    public static float GetStringHeight(Text text)
    {
        float h = text.cachedTextGenerator.GetPreferredHeight(text.text, text.GetGenerationSettings(text.rectTransform.rect.size));
        //计算字符串高度在不同分辨率的屏幕下所得出的大小不一样，需要以基础的分辨率来进行缩放。
        return h * UIRoot.instatnce.scale;
    }

    public static void SetButtonActive(Button btn, bool b)
    {
        if (btn != null)
        {
            btn.interactable = b;
            Text text = btn.GetComponentInChildren<Text>();
            if (text == null)
            {
                return;
            }

            if (b)
            {
                text.color = new Color(0.77f, 0.77f, 0.77f);
            }
            else
            {
                text.color = Color.gray;
            }
        }
    }
}