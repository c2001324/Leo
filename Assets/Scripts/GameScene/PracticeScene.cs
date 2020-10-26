using UnityEngine;
using UnityEditor;

public class PracticeScene : GameScene
{
    public override GameSceneType type { get { return GameSceneType.Practice; } }

    public override void EnterScene()
    {
        EnterComplete();
    }

    public override void ExitScene()
    {
        ExitComplete();
    }
}