using UnityEngine;
using UnityEditor;
using System;

public class TitleScene : GameScene
{
    public override GameSceneType type { get { return GameSceneType.Title; } }

    public override void EnterScene()
    {
        UIManager.instance.OpenUI<UITitleWnd>(false);
        EnterComplete();
    }

    public override void ExitScene()
    {
        UIManager.instance.CloseUI<UITitleWnd>();
        ExitComplete();
    }
}