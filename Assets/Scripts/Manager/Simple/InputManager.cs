using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Untility;
using HexMap;

/// <summary>
/// 控制器
/// </summary>
public class InputManager : Singleton<InputManager>, ISimpleInitManager
{
    public bool active
    {
        get
        {
            return GameSceneManager.instance.curGameScene != GameSceneType.ExitGame;
        }
    }

    public InputController controller { get; private set; }

    public void Initialize()
    {
        
    }

    public void SetInputController(InputController controller)
    {
        this.controller = controller;
    }

    public void RemoveController()
    {
        controller = null;
    }

    public bool IsOverlapUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }


    

    public void Update()
    {
        if (!active)
        {
            return;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            UICombatMenuWnd wnd = UIManager.instance.GetUI<UICombatMenuWnd>();
            if (wnd.isShow)
            {
                UIManager.instance.CloseUI<UICombatMenuWnd>();
            }
            else
            {
                UIManager.instance.OpenUI<UICombatMenuWnd>(false);
            }
        }

        if (Input.GetKeyUp(KeyCode.BackQuote))
        {
            if (UIConsoleWnd.instance.isShow)
            {
                UIManager.instance.CloseUI<UIConsoleWnd>();
            }
            else
            {
                UIManager.instance.OpenUI<UIConsoleWnd>(false);
            }
        }
    }
}
