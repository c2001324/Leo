using UnityEngine;
using UnityEditor;

public class PreCombatScene : GameScene
{
    public override GameSceneType type { get { return GameSceneType.PreCombat; } }

    protected override void OnEnterScene(GameSceneParam param)
    {
        EnterComplete(null);
    }

    protected override void OnExitScene(GameSceneParam param)
    {
        ExitComplete(null);
    }
}