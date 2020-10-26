using UnityEngine;
using UnityEditor;

public class ExitGameScene : GameScene
{
    public override GameSceneType type { get { return GameSceneType.ExitGame; } }

    public override void EnterScene()
    {
        EnterComplete();
    }

    public override void ExitScene()
    {
        ExitComplete();
    }
}