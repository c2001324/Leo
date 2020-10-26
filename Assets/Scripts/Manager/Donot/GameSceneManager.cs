using System.Collections.Generic;
using UnityEngine;
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

    public GameSceneType curGameScene { get { return m_CurGameScene.type; } }

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

    public void EnterGameScene(GameSceneType nextScene)
    {
        EnterGameScene(GetGameScene(nextScene));
    }

    public void EnterGameScene(GameScene nextScene)
    {
        if (state == State.Done)
        {
            state = State.ExitGameScene;
            m_NextGameScene = nextScene;
            GameEvent.Scene.FireOnBeginExitScene(m_CurGameScene.type);
            m_CurGameScene.ExitScene();
        }
    }

    void OnEnterComplete(GameScene gameScene)
    {
        m_CurGameScene = m_NextGameScene;
        GameEvent.Scene.FireOnEnterSceneComplete(gameScene.type);
        state = State.Done;
        m_NextGameScene = null;
    }

    void OnExitComplete(GameScene gameScene)
    {
        state = State.EnterGameScene;
        GameEvent.Scene.FireOnExitSceneComplete(m_CurGameScene.type);
        GameEvent.Scene.FireOnBeginEnterScene(m_NextGameScene.type);
        m_NextGameScene.EnterScene();
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