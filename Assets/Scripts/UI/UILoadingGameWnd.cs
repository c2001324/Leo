using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 处理所有的场景切换逻辑
/// </summary>
public class UILoadingGameWnd : UIWindow
{
    
    protected override void afterCreate()
    {
        
    }

    protected override bool beforeOpen(object param1 = null, object param2 = null, object param3 = null)
    {
        GameManager.instance.onLoadingGame.AddListener(OnLoadingGame);
        return true;
    }
    
    void OnLoadingGame(string str, float p)
    {

    }


    protected override void beforeClose()
    {
        GameManager.instance.onLoadingGame.RemoveListener(OnLoadingGame);
    }

    
}
