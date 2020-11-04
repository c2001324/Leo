using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICombatMainEntityTurnItemContainer : MonoBehaviour
{
    public UICombatMainEntityTurnItem prefab;

    public Transform turnSplit;

    List<UICombatMainEntityTurnItem> m_Items = new List<UICombatMainEntityTurnItem>();

    public void Initialize()
    {
        prefab.gameObject.SetActive(false);
    }

    public void Open()
    {
        GameEvent.TurnBaseEvent.onEntityTurnStart.AddListener(OnEntityTurnStart);
    }

    public void Close()
    {
        ResetAllItems();
        GameEvent.TurnBaseEvent.onEntityTurnStart.RemoveListener(OnEntityTurnStart);
    }


    void OnEntityTurnStart(CControllerComponent t)
    {
        ResetAllItems();
        UpdateEntityTurnList(TurnBaseManager.instance.curTurnBase, TurnBaseManager.instance.nextTurnBase);
    }

    void UpdateEntityTurnList(TurnBase turnBase, TurnBase nextTurnBase)
    {
        UICombatMainEntityTurnItem item = GetItem();
        item.SetController(turnBase.curController);
        foreach (CControllerComponent t in turnBase.readyControllerList)
        {
            item = GetItem();
            item.SetController(t);
        }
        turnSplit.SetSiblingIndex(item.transform.GetSiblingIndex());
        foreach (CControllerComponent t in nextTurnBase.readyControllerList)
        {
            GetItem().SetController(t);
        }
    }

    UICombatMainEntityTurnItem GetItem()
    {
        foreach (UICombatMainEntityTurnItem item in m_Items)
        {
            if (!item.gameObject.activeSelf)
            {
                return item;
            }
        }

        GameObject obj = GameObject.Instantiate<GameObject>(prefab.gameObject);
        obj.transform.parent = transform;
        UICombatMainEntityTurnItem newItem = obj.GetComponent<UICombatMainEntityTurnItem>();
        newItem.Initialize();
        newItem.ResetController();
        m_Items.Add(newItem);
        return newItem;
    }

    void ResetAllItems()
    {
        foreach (UICombatMainEntityTurnItem item in m_Items)
        {
            item.ResetController();
        }
    }
}
