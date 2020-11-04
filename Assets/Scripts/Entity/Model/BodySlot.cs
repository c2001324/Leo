using System.Collections.Generic;
using UnityEngine;
using System;
using HighlightingSystem;

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