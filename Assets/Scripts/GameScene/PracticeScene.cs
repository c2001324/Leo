public class PracticeScene : GameScene
{
    public override GameSceneType type { get { return GameSceneType.Practice; } }

    GameSceneParam m_Param;

    protected override void OnEnterScene(GameSceneParam param)
    {
        LevelManager.instance.BuildLevel(-1, GameManager.instance.seed);
        GameEvent.LevelEvent.onBuildLevelComplete.AddListener(OnBuildLevelComplete);
        GameEvent.RoomEvent.onEnterRoom.AddListener(OnEnterRoom);
        GameEvent.RoomEvent.onBeforeEnterRoom.AddListener(OnBeforeEnterRoom);
    }

    void OnBuildLevelComplete(LevelInstance level)
    {
        LevelManager.instance.EnterLevel(level.levelIndex);   
    }

    void OnBeforeEnterRoom(RoomInstance room)
    {
        PlayerManager.instance.InitilaizePlayer(room);
        UIManager.instance.OpenUI<UICombatMainWnd>(false);
        UIManager.instance.OpenUI<UIHeadWnd>(false);
    }

    void OnEnterRoom(RoomInstance room)
    {
        GameEvent.LevelEvent.onBuildLevelComplete.RemoveListener(OnBuildLevelComplete);
        GameEvent.RoomEvent.onEnterRoom.RemoveListener(OnEnterRoom);
        GameEvent.RoomEvent.onBeforeEnterRoom.RemoveListener(OnBeforeEnterRoom);
        EnterComplete(null);
    }

    protected override void OnExitScene(GameSceneParam param)
    {
        GameEvent.LevelEvent.onExitLevel.AddListener(OnExitLevel);
        GameEvent.LevelEvent.onDestroyLevelComplete.AddListener(OnDestroyLevelComplete);

        m_Param =param;
        LevelManager.instance.ExitLevel(m_Param.exitLevelType);
    }



    void OnExitLevel(LevelInstance level, ExitLevelType type)
    {
        LevelManager.instance.DestroyLevel(level.levelIndex, type);
        
    }

    void OnDestroyLevelComplete(LevelInstance level, ExitLevelType type)
    {
        GameEvent.LevelEvent.onExitLevel.RemoveListener(OnExitLevel);
        GameEvent.LevelEvent.onDestroyLevelComplete.RemoveListener(OnDestroyLevelComplete);
        ExitComplete(m_Param);
        m_Param = null;
    }
}
