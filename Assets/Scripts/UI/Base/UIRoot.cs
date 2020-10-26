using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UIRoot : MonoBehaviour
{

    public static UIRoot Create()
    {
        GameObject obj = GameObject.Find("UIRoot");
        if (obj == null)
        {
            obj = Resources.Load<GameObject>("UI/Base/UIRoot");
            obj = GameObject.Instantiate<GameObject>(obj);
            obj.name = "UIRoot";
        }
        instatnce = obj.GetComponent<UIRoot>();
        instatnce.Initializ();
        return instatnce;
    }

    static public UIRoot instatnce { get; private set; }


    
    public Canvas canvas { get; private set; }


    public GraphicRaycaster graphicRaycaster { get { return m_GraphicRaycaster; } }
    GraphicRaycaster m_GraphicRaycaster;

    /// <summary>
    /// 基础的分辨率
    /// </summary>
    static Vector2 baseResolution = new Vector2(1536f, 2048f);
    static Vector2 baseResolutionWithWidth = new Vector2(1152f, 2048f);

    /// <summary>
    /// 屏幕的分辨率
    /// </summary>
    public static Vector2 screenResolution { get { return m_ScreenResolution; } }
    static Vector2 m_ScreenResolution;

    Camera m_UICamera;

    /// <summary>
    /// 屏幕的缩放
    /// </summary>
    public float scale { get { return m_Scale; } }
    float m_Scale;

    /// <summary>
    /// 分辨率
    /// </summary>
    public enum ResolutionType
    {
        Resolution_1920_1080,
        Resolution_1600_900,
        Resolution_1280_720,
        None
    }

    //默认分辨率
    public const ResolutionType defResolution = ResolutionType.Resolution_1920_1080;

    //当前分辨率
    protected ResolutionType m_CurResolutionType;



    //原图的压缩比率
    public float compressRotio
    {
        get
        {
            if (scaler == null)
            {
                return 1;
            }
            else
            {
                return scaler.scaleFactor;
            }
        }
    }


    public CanvasScaler scaler { get; private set; }


    public void SetResolution(ResolutionType type, bool bFullScreen)
    {
        int width = 0;
        int height = 0;
        switch (type)
        {
            case ResolutionType.Resolution_1920_1080:
                width = 1920;
                height = 1080;
                break;
            case ResolutionType.Resolution_1600_900:
                width = 1600;
                height = 900;
                break;
            case ResolutionType.Resolution_1280_720:
                width = 1280;
                height = 720;
                break;
            default:
                Application.Quit();
                return;
        }

        try
        {
            m_CurResolutionType = type;

            Screen.SetResolution(width, height, bFullScreen);
            scaler.referenceResolution = new Vector2(width, height);
            scaler.scaleFactor = (float)height / 1080f;
            scaler.referencePixelsPerUnit = 100;

            Invoke("ResetCursor", 0.1f);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// 重置光标
    /// </summary>
    void ResetCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Initializ()
    {
        DontDestroyOnLoad(this);
        m_GraphicRaycaster = gameObject.GetComponent<GraphicRaycaster>();
        scaler = gameObject.GetComponent<CanvasScaler>();
        canvas = gameObject.GetComponent<Canvas>();
        m_ScreenResolution = new Vector2(Screen.width, Screen.height);
        gameObject.SetActive(true);
    }

}

