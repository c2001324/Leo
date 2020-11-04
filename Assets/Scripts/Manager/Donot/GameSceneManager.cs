using System.Collections.Generic;
using Untility;

/// <summary>
/// 场景 管理器
/// </summary>
public class GameSceneManager : Singleton<GameSceneManager>, IDonotInitManager
{
    /// <summary>
    /// 初始化游戏场景
    /// </summary>
    public void InitializeGameScene(GameSceneType defaultGameScene)
    {
        m_CurGameScene = GetGameScene(GameSceneType.ExitGame);
        state = State.Done;
        m_NextGameScene = null;

        if (defaultGameScene == GameSceneType.ExitGame)
        {
            defaultGameScene = GameSceneType.Title;
        }
        EnterGameScene(defaultGameScene);
    }

    public GameSceneType oldGameScene
    {
        get
        {
            if (m_OldGameScene == null)
            {
                return GameSceneType.ExitGame;
            }
            return m_OldGameScene.type;
        }
    }

    public GameSceneType curGameScene
    {
        get
        {
            if (m_CurGameScene == null)
            {
                return GameSceneType.ExitGame;
            }
            return m_CurGameScene.type;
        }
    }

    GameScene m_OldGameScene;

    GameScene m_CurGameScene;

    GameScene m_NextGameScene;

    public enum State
    {
        Done,
        EnterGameScene,
        ExitGameScene
    }
    public State state { get; private set; }

    public void EnterGameScene(GameSceneType nextScene, GameSceneParam param = null)
    {
        EnterGameScene(GetGameScene(nextScene), param);
    }

    void EnterGameScene(GameScene nextScene, GameSceneParam param)
    {
        if (state == State.Done)
        {
            state = State.ExitGameScene;
            m_NextGameScene = nextScene;
            GameEvent.SceneEvent.FireOnBeginExitScene(m_CurGameScene.type);
            m_CurGameScene.ExitScene(param);
        }
    }

    void OnEnterComplete(GameScene gameScene, GameSceneParam param)
    {
        m_OldGameScene = m_CurGameScene;
        m_CurGameScene = m_NextGameScene;
        m_NextGameScene = null;
        GameEvent.SceneEvent.FireOnEnterSceneComplete(gameScene.type);
        state = State.Done;   
    }

    void OnExitComplete(GameScene gameScene, GameSceneParam param)
    {
        state = State.EnterGameScene;
        GameEvent.SceneEvent.FireOnExitSceneComplete(m_CurGameScene.type);
        GameEvent.SceneEvent.FireOnBeginEnterScene(m_NextGameScene.type);
        m_NextGameScene.EnterScene(param);
    }


    Dictionary<GameSceneType, GameScene> m_GameScenes = new Dictionary<GameSceneType, GameScene>();

    GameScene GetGameScene(GameSceneType type)
    {
        GameScene gameScene;
        if (!m_GameScenes.TryGetValue(type, out gameScene))
        {
            switch (type)
            {
                case GameSceneType.Combat:
                    gameScene = new CombatScene();
                    break;
                case GameSceneType.Title:
                    gameScene = new TitleScene();
                    break;
                case GameSceneType.PreCombat:
                    gameScene = new PreCombatScene();
                    break;
                case GameSceneType.Practice:
                    gameScene = new PracticeScene();
                    break;
                default:
                    gameScene = new ExitGameScene();
                    break;
            }
            gameScene.onEnterComplete += OnEnterComplete;
            gameScene.onExitComplete += OnExitComplete;
            m_GameScenes.Add(type, gameScene);
        }
        return gameScene;
    }

}