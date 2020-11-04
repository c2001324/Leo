using UnityEngine;
using UnityEngine.UI;


public class UICombatMenuWnd : UIWindow
{
   
    Button m_BtnRestart;
    Button m_BtnExitLevel;
    Button m_BtnRetureToTitle;
    Button m_BtnExitGame;
    Button m_BtnReturn;

    protected override void afterCreate()
    {
        m_BtnRestart = transform.Find("BtnRestart").GetComponent<Button>();
        m_BtnExitLevel = transform.Find("BtnExitLevel").GetComponent<Button>();
        m_BtnRetureToTitle = transform.Find("BtnReturmToTitle").GetComponent<Button>();
        m_BtnExitGame = transform.Find("BtnExitGame").GetComponent<Button>();
        m_BtnReturn = transform.Find("BtnReturn").GetComponent<Button>();

        m_BtnRestart.onClick.AddListener(OnRestart);
        m_BtnExitLevel.onClick.AddListener(OnExitLevel);
        m_BtnRetureToTitle.onClick.AddListener(OnRetureToTitle);
        m_BtnExitGame.onClick.AddListener(OnExitGame);
        m_BtnReturn.onClick.AddListener(OnReturn);   
    }

    protected override bool beforeOpen(object param1 = null, object param2 = null, object param3 = null)
    {
        if (GameSceneManager.instance.curGameScene == GameSceneType.Combat)
        {
            m_BtnRestart.interactable = true;
            m_BtnExitLevel.interactable = true;
        }
        else if (GameSceneManager.instance.curGameScene == GameSceneType.Practice)
        {
            m_BtnRestart.interactable = false;
            m_BtnExitLevel.interactable = true;
        }
        else
        {
            m_BtnRestart.interactable = false;
            m_BtnExitLevel.interactable = false;
        }
        return true;
    }

    protected override void afterOpen()
    {
        m_BtnReturn.Select();
        Time.timeScale = 0f;
    }

    protected override void beforeClose()
    {
        Time.timeScale = 1f;
    }


    void OnRestart()
    {
    }

    void OnExitLevel()
    {
        GameSceneManager.instance.EnterGameScene(GameSceneType.Title, new GameSceneParam() { exitLevelType = ExitLevelType.ExitLevel });
    }

    void OnRetureToTitle()
    {
    }

    void OnExitGame()
    {
    }

    void OnReturn()
    {
        Close();
    }
}
