public class CombatScene : GameScene
{
    public override GameSceneType type { get { return GameSceneType.Combat; } }

    GameSceneParam m_Param;

    protected override void OnEnterScene(GameSceneParam param)
    {
        m_Param = param;
        LevelManager.instance.BuildLevel(-1, GameManager.instance.seed);
        GameEvent.LevelEvent.onBuildLevelComplete.AddListener(OnEnterLevel);
    }

    void OnEnterLevel(LevelInstance level)
    {
        GameEvent.LevelEvent.onBuildLevelComplete.RemoveListener(OnEnterLevel);
        EnterComplete(m_Param);
        m_Param = null;
    }

    protected override void OnExitScene(GameSceneParam param)
    {
        m_Param = param;
        GameEvent.LevelEvent.onDestroyLevelComplete.AddListener(OnDestroyLevelComplete);
    }

    void OnDestroyLevelComplete(LevelInstance level, ExitLevelType type)
    {
        GameEvent.LevelEvent.onDestroyLevelComplete.RemoveListener(OnDestroyLevelComplete);
        ExitComplete(m_Param);
        m_Param = null;
    }
}