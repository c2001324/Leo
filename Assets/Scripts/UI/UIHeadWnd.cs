using UnityEngine;

/// <summary>
/// 战斗中人物头顶的UI
/// </summary>
public class UIHeadWnd : UIWindow
{
    public static UIHeadWnd instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = UIManager.instance.GetUI<UIHeadWnd>();
            }
            return m_Instance;
        }
    }
    static UIHeadWnd m_Instance;


    public UIAvatarHead CreateAvatarHeadUI(EAvatar avatar)
    {
        UIAvatarHead ui = UIAvatarHead.Create();
        ui.transform.SetParent(transform, false);
        ui.SetAvatar(avatar);
        return ui;
    }
    
    public void DestoryHeadUI(UIFollowEntity ui)
    {
        if (ui != null)
        {
            ui.ResetFollowTarget();
            ui.ResetData();
            GameObject.Destroy(ui.gameObject);
        }
    }
}
