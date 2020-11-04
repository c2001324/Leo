using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICombatMainPlayerItemContainer : MonoBehaviour
{
    public UICombatMainPlayerItem prefab;

    List<UICombatMainPlayerItem> m_Items = new List<UICombatMainPlayerItem>();

    public void Initialize()
    {
        prefab.gameObject.SetActive(false);
    }

    public void Open()
    {
        foreach (EAvatar avatar in PlayerManager.instance.avatars)
        {
            GetPlayerItem().SetPlayer(avatar);
        }
    }

    public void Close()
    {
        foreach (UICombatMainPlayerItem item in m_Items)
        {
            item.ResetPlayer();
        }
    }

    UICombatMainPlayerItem GetPlayerItem()
    {
        foreach (UICombatMainPlayerItem item in m_Items)
        {
            if (!item.gameObject.activeSelf)
            {
                return item;
            }
        }

        GameObject obj = GameObject.Instantiate<GameObject>(prefab.gameObject);
        obj.transform.parent = transform;
        UICombatMainPlayerItem newItem = obj.GetComponent<UICombatMainPlayerItem>();
        newItem.Initialize();
        newItem.ResetPlayer();
        m_Items.Add(newItem);
        return newItem;
    }
}
