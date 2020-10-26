using System.Collections.Generic;
using UnityEngine;
using System;

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

    public void Intialize()
    {
        if (!m_HasInit)
        {
            slot.Initilaize();
            UpdateRendererObjects();
            OnIntialize();
            m_HasInit = true;
        }
    }


    protected virtual void OnIntialize()
    {

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
        EntityFlagsChanged(m.entity.flags);
        return true;
    }

    public void EntityFlagsChanged(EntityFlags flags)
    {
        if (Entity.CheckFlags(flags, EntityFlags.NoColliderWithEntity))
        {
            entity.gameObject.layer = LayerMask.NameToLayer(Layers.noColliderWithEntity);
        }
        else
        {
            entity.gameObject.layer = LayerMask.NameToLayer(Layers.entity);
        }
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

}

[System.Serializable]
public class BodySlot
{
    //头顶的UI
    public Transform headUI;
    //头部
    public Transform head;
    //左手
    public Transform leftHand;
    //右手
    public Transform rightHand;
    //中心腰部
    public Transform pelvis;
    //左脚
    public Transform leftFoot;
    //右脚
    public Transform rightFoot;
    //击中的部位
    public Transform hitLocation;
    //位置
    public Transform origin;
    //射击的位置
    public Transform projectile;

    //武器Slot
    public Transform wand;
    public Transform twoHandSword;
    public Transform sword_R;
    public Transform shield;
    public Transform bow;

    Dictionary<BodySlotType, Transform> m_Slots = new Dictionary<BodySlotType, Transform>();

    public void Initilaize()
    {
        m_Slots.Add(BodySlotType.HeadUI, headUI);
        m_Slots.Add(BodySlotType.Head, head);
        m_Slots.Add(BodySlotType.LeftHand, leftHand);
        m_Slots.Add(BodySlotType.RightHand, rightHand);
        m_Slots.Add(BodySlotType.Pelvis, pelvis);
        m_Slots.Add(BodySlotType.LeftFood, leftFoot);
        m_Slots.Add(BodySlotType.RightFoor, rightFoot);
        m_Slots.Add(BodySlotType.Hit, hitLocation);
        m_Slots.Add(BodySlotType.Origin, origin);
        m_Slots.Add(BodySlotType.Projectile, projectile);

        //保证projectile的高度统一
        if (projectile != null)
        {
            projectile.position = new Vector3(projectile.position.x, 1f, projectile.position.z);
        }
    }

    public Transform[] GetWeaponSlot(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.TwoHandSword:
                return new Transform[] { twoHandSword };
            case WeaponType.SwordShield:
                return new Transform[] { sword_R, shield };
            case WeaponType.Bow:
                return new Transform[] { bow };
            default:
                return new Transform[] { wand };
        }
    }

    public static bool CheckBodySlotType(BodySlotType source, BodySlotType target)
    {
        return (source & target) == target;
    }

    public Transform GetBodySlot(BodySlotType type)
    {
        return m_Slots[type];
    }

    public Transform[] GetBodySlots(BodySlotType type)
    {
        List<Transform> list = new List<Transform>();
        foreach (BodySlotType targetType in Enum.GetValues(typeof(BodySlot)))
        {
            if (CheckBodySlotType(type, targetType) && m_Slots[targetType] != null)
            {
                list.Add(m_Slots[targetType]);
            }
        }
        return list.ToArray();
    }
}

[Flags]
public enum BodySlotType
{
    None = 0,
    HeadUI = 1 << 1,
    Head = 1 << 2,
    LeftHand = 1 << 3,
    RightHand = 1 << 4,
    Pelvis = 1 << 5,
    LeftFood = 1 << 6,
    RightFoor = 1 << 7,
    Hit = 1 << 8,
    Origin = 1 << 9,
    Projectile = 1 << 10,
}