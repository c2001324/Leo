using UnityEngine;
using UnityEditor;

public class PreCombatScene : GameScene
{
    public override GameSceneType type { get { return GameSceneType.PreCombat; } }

    public override void EnterScene()
    {
        EnterComplete();
    }

    public override void ExitScene()
    {
        ExitComplete();
    }
}