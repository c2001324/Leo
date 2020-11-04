using UnityEngine;
using UnityEditor;

public class ExitGameScene : GameScene
{
    public override GameSceneType type { get { return GameSceneType.ExitGame; } }

    protected override void OnEnterScene(GameSceneParam param)
    {
        EnterComplete(null);
    }

    protected override void OnExitScene(GameSceneParam param)
    {
        ExitComplete(null);
    }
}