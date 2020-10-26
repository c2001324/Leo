using UnityEngine;
using UnityEditor;

public class CombatScene : GameScene
{
    public override GameSceneType type { get { return GameSceneType.Combat; } }

    public override void EnterScene()
    {
        EnterComplete();
    }

    public override void ExitScene()
    {
        ExitComplete();
    }
}