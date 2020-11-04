using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏管理器，游戏的入口
/// </summary>
public class GameManager : MonoBehaviour
{

    public static GameManager instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindGameObjectWithTag(Tags.gameManager).GetComponent<GameManager>();
            }
            return m_Instance;
        }
    }
    static GameManager m_Instance;

    public GameSceneType defaultGameScene;

    public int levelId;
    public int areaIndex;
    public int seed;
    public bool randomSeed;

    public bool monsterAI = true;

    public bool log = false;

    [Range(0f, 2f)]
    public float volume = 1;

    
    private void Awake()
    {
        //Cursor.visible = false;
        if (log)
        {
            Untility.Log.instance.Initialize(LogType.Error, true, false);
        }
        m_Instance = this;
        GameObject.DontDestroyOnLoad(this);
        //初始化三个基础管理器
        InitilaizeBaseManager();
        //开始初始化，出现界面
        StartCoroutine(LoadingGame());
    }


    private void OnDestroy()
    {
        //LevelManager.instance.UnloadLevel(Level.UnloadLevelType.Close);
    }

    IEnumerator LoadingGame()
    {
        //单独初始化UIManager
        foreach (ManagerProgress p in UIManager.instance.Initialize())
        {
            yield return null;
        }
        UIManager.instance.OpenUI<UILoadingGameWnd>(false);
        yield return null;
        //初始化普通管理器
        InitializeSimpleManager();
        //注册异步的管理器
        RegisterAllSyncManager();
        yield return null;
        //初始化Sync管理器
        foreach (ManagerProgress p in InitlaizeSyncManager())
        {
            onLoadingGame.Invoke(p.describe, p.progress);
            yield return null;
        }
        UIManager.instance.CloseUI<UILoadingGameWnd>();

        if (randomSeed)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        GameSceneManager.instance.InitializeGameScene(defaultGameScene);
    }

    #region Base管理器
    void InitilaizeBaseManager()
    {
        TimerManager.Initialize(gameObject);
    }
    #endregion

    #region Sync管理器
    List<SyncManagerData> m_SyncManagers = new List<SyncManagerData>();
    float m_TotalWeight;
    class SyncManagerData
    {
        public SyncManagerData(ISyncInitManager m, float weight)
        {
            this.weight = weight;
            manager = m;
        }
        public ISyncInitManager manager;
        public float weight;
    }

    int  count = 0;

    void RegisterSyncManager(ISyncInitManager m, float weight)
    {
        count++;
        if (m == null)
        {
            Debug.LogError("为空" + count);
        }
        if (!SyncManagerIsExists(m))
        {
            m_SyncManagers.Add(new SyncManagerData(m, weight));
            m_TotalWeight += weight;
        }
    }

    bool SyncManagerIsExists(ISyncInitManager m)
    {
        foreach (SyncManagerData d in m_SyncManagers)
        {
            if (d.manager == m)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerable<ManagerProgress> InitlaizeSyncManager()
    {
        float baseProgress = 0f;
        foreach (SyncManagerData data in m_SyncManagers)
        {
            foreach (ManagerProgress p in data.manager.Initialize())
            {
                baseProgress += p.progress * data.weight / m_TotalWeight;
                yield return new ManagerProgress(baseProgress, p.describe);
            }
        }
    }

    void RegisterAllSyncManager()
    {
        float baseWeight = 10f;
        RegisterSyncManager(ConfigManager.instance, baseWeight);
        RegisterSyncManager(IconManager.instance, baseWeight);
        RegisterSyncManager(CheatCodeManager.instance, baseWeight);
        RegisterSyncManager(RoomManager.instance, baseWeight);
    }
    #endregion

    #region Simple管理器
    void InitializeSimpleManager()
    {
        InputManager.instance.Initialize();
        CustomTextManager.instance.Initialize();
        EntityManager.instance.Initialize();
        EntityModelManager.instance.Initialize();
        LevelManager.instance.Initialize();
        TurnBaseManager.instance.Initialize();
        PlayerManager.instance.Initialize();
        CameraManager.instance.Initialize();
    }
    #endregion

    private void Update()
    {
        InputManager.instance.Update();
    }

    private void LateUpdate()
    {
    }

    public void SlowDown(float value, float durationTime)
    {
        DOTween.To(() => Time.timeScale, (x) => Time.timeScale = x, value, durationTime).OnComplete(OnComplete);
    }

    void OnComplete()
    {
        DOTween.To(() => Time.timeScale, (x) => Time.timeScale = x, 1f, 0.2f);
    }

    void OnLoading(int step)
    {
        onLoadingGame.Invoke("加载管理器", (float)step / 28f);
    }

    readonly public LoadingGameEvent onLoadingGame = new LoadingGameEvent();

    public class LoadingGameEvent : CustomEvent<string, float> { }
}
