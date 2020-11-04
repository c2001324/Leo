using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class UICombatMainWnd : UIWindow
{

    UICombatMainPlayerItemContainer m_PlayerContainer;

    UICombatMainEntityTurnItemContainer m_EntityTurnContainer;

    UICombatMainController m_ControllerPanel;


    protected override void afterCreate()
    {
        m_PlayerContainer = transform.Find("PlayerList").GetComponent<UICombatMainPlayerItemContainer>();
        m_PlayerContainer.Initialize();

        m_EntityTurnContainer = transform.Find("EntityTurnList").GetComponent<UICombatMainEntityTurnItemContainer>();
        m_EntityTurnContainer.Initialize();

        m_ControllerPanel = transform.Find("Controller").GetComponent<UICombatMainController>();
        m_ControllerPanel.Initialize();
    }

    protected override bool beforeOpen(object param1 = null, object param2 = null, object param3 = null)
    {
        m_PlayerContainer.Open();
        m_EntityTurnContainer.Open();
        m_ControllerPanel.Open();
        return true;
    }
    
    protected override void beforeClose()
    {
        m_PlayerContainer.Close();
        m_EntityTurnContainer.Close();
        m_ControllerPanel.Close();
    }
}
