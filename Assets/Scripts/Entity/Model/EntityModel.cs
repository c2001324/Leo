using System.Collections.Generic;
using UnityEngine;
using System;
using HighlightingSystem;

/// <summary>
/// 模型基础类
/// 由EntityModelManager 创建和管理
/// </summary>
public class EntityModel : MonoBehaviour
{
    #region 创建Entity的资源，由Editor调用
    public static void SetEntityModelResource(EntityModel m, bool canSelected)
    {
        //m.gameObject.layer = LayerMask.NameToLayer(Layers.entity);
        if (canSelected)
        {
            m.gameObject.AddComponent<BoxCollider>();
        }

        #region 添加HeadUI
        GameObject headUI = new GameObject("HeadUI");
        headUI.transform.parent = m.transform;
        m.slot.headUI = headUI.transform;
        #endregion
    }
    #endregion

    public BodySlot slot = new BodySlot();

    #region 渲染相关

    List<Renderer> m_AllRenderers = new List<Renderer>();

    Dictionary<Renderer, Material[]> m_SourceMaterialData = new Dictionary<Renderer, Material[]>();

    /// <summary>
    /// 渲染物体更新
    /// </summary>
    public void UpdateRendererObjects()
    {
        List<Renderer> newRenderer = new List<Renderer>(transform.GetComponentsInChildren<Renderer>());
        //删除旧的材质
        foreach (Renderer r in m_AllRenderers)
        {
            if (!newRenderer.Contains(r))
            {
                m_SourceMaterialData.Remove(r);
            }
        }
        //添加新的材质
        foreach (Renderer r in newRenderer)
        {
            if (!m_SourceMaterialData.ContainsKey(r))
            {
                m_SourceMaterialData.Add(r, r.materials);
            }
        }
        m_AllRenderers = newRenderer;
    }

   

    public bool visiable
    {
        get
        {
            return m_Visiable;
        }
        set
        {
            
            if (value != m_Visiable)
            {
                if (value)
                {
                    ResetMaterial();
                    shadow = true;
                }
                else
                {
                    SetMaterial(Shader.Find("Custom/TransparentEffect"), new Color(1f, 1f, 1f, 0f), false);
                    shadow = false;
                }
            }
            m_Visiable = value;
        }
    }
    bool m_Visiable = true;

    public bool transparent
    {
        get
        {
            return m_Transparent;
        }
        set
        {
            if (value != m_Transparent)
            {
                if (!value)
                {
                    ResetMaterial();
                }
                else
                {
                    SetMaterial(Shader.Find("Custom/TransparentEffect"), new Color(1f, 1f, 1f, 0.3f), true);
                }
                m_Transparent = value;
            }
        }
    }

    bool m_Transparent = false;

    public bool shadow
    {
        get
        {
            foreach (Renderer r in m_AllRenderers)
            {
                return r.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            return false;
        }
        set
        {
            foreach (Renderer r in m_AllRenderers)
            {
                if (value)
                {
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                else
                {
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }
        }
    }

    /// <summary>
    /// 设置为原来的材质
    /// </summary>
    void ResetMaterial()
    {
        foreach (Renderer r in m_AllRenderers)
        {
            r.materials = m_SourceMaterialData[r];
        }
    }

    /// <summary>
    /// 设置新的材质
    /// </summary>
    /// <param name="mat"></param>
    void SetMaterial(Shader shader, Color color, bool useSourceTexture)
    {
        foreach (Renderer r in m_AllRenderers)
        {
            Material[] mats = new Material[r.materials.Length];
            for (int matIndex = 0; matIndex < mats.Length; matIndex++)
            {
                Material mat = new Material(shader);
                mat.color = color;
                if (useSourceTexture)
                {
                    mat.mainTexture = m_SourceMaterialData[r][matIndex].mainTexture;
                }
                mats[matIndex] = mat;
            }
            r.materials = mats;
        }
    }


    #endregion

    public ModelComponent modelComponent { get; private set; }

    

    public Entity entity
    {
        get
        {
            if (modelComponent == null)
            {
                return null;
            }
            else
            {
                return modelComponent.entity;
            }
        }
    }

    bool m_HasInit = false;

    public Highlighter highlighter { get; private set; }

    public Animator GetAnimator()
    {
        return gameObject.GetComponent<Animator>();
    }

    public void Intialize()
    {
        if (!m_HasInit)
        {
            highlighter = gameObject.GetComponent<Highlighter>();
            if (highlighter == null)
            {
                highlighter = gameObject.AddComponent<Highlighter>();
                highlighter.overlay = true;
            }
            slot.Initilaize();
            UpdateRendererObjects();
            OnIntialize();
            m_HasInit = true;
        }
    }


    protected virtual void OnIntialize()
    {
        
    }

    public void SetHighlight(Color color)
    {
        highlighter.constantColor = color;
        highlighter.constant = true;
    }

    public void SetHighlight(Relation relation)
    {
        SetHighlight(Untility.TextColor.GetHighlight(relation));
    }

    public void CloseHighlight()
    {
        highlighter.constant = false;
    }

    /// <summary>
    /// 由ModelComponent 调用
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public bool AttachToModelComponent(ModelComponent m)
    {
        Intialize();
        if (m != null)
        {
            DeattachFromModelComponent();
            transform.SetParent(m.entity.transform, false);
            modelComponent = m;
            return OnAttachToModelComponent(m);
        }
        else
        {
            return false;
        }
    }

    protected virtual bool OnAttachToModelComponent(ModelComponent m)
    {
        return true;
    }


    /// <summary>
    /// 由ModelComponent 调用
    /// </summary>
    public void DeattachFromModelComponent()
    {
        if (modelComponent != null)
        {
            transform.parent = null;
            modelComponent = null;
            OnDeattachFromModelComponent();
        }
    }

    protected virtual void OnDeattachFromModelComponent()
    {

    }

    void OnMouseEnter()
    {
        if (!InputManager.instance.IsOverlapUI() && modelComponent != null)
        {
            modelComponent.Select(EntityModelSelectType.FromModel);
        }
    }

    void OnMouseExit()
    {
        if (!InputManager.instance.IsOverlapUI() && modelComponent != null)
        {
            modelComponent.UnSelect(EntityModelSelectType.FromModel);
        }
    }

    void OnMouseOver()
    {
        if (!InputManager.instance.IsOverlapUI() && modelComponent != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                modelComponent.LeftClick(EntityModelSelectType.FromModel);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                modelComponent.RightClick(EntityModelSelectType.FromModel);
            }
        }
    }
}