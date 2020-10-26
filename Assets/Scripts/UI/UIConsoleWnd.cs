using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIConsoleWnd : UIWindow
{
    InputField m_Input;

    public static UIConsoleWnd instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = UIManager.instance.GetUI<UIConsoleWnd>();
            }
            return m_Instance;
        }
    }
    static UIConsoleWnd m_Instance;

    //旧的输入列表
    List<string> m_OldCmd = new List<string>();
    int m_index;


    ScrollRect m_TextScrollRect;
    Text m_Text;

    int m_CurTextIndex;

    protected override void afterCreate()
    {
        m_Instance = this;
        m_Input = transform.Find("InputField").GetComponent<InputField>();
        m_Input.onEndEdit.AddListener(OnEndEdit);
        m_Input.onValueChanged.AddListener(OnValueChanged);

        m_TextScrollRect = transform.Find("Info").GetComponent<ScrollRect>();
        m_Text = m_TextScrollRect.content.Find("Text").GetComponent<Text>();
    }

    public void Show()
    {
        if (isShow)
        {
            Close();
        }
        else
        {
            Open(true);
        }
    }



    protected override bool beforeOpen(object param1 = null, object param2 = null, object param3 = null)
    {
        ResetMsg();
        Time.timeScale = 0;
        return true;
    }

    protected override void afterOpen()
    {
        m_Input.ActivateInputField();
        base.afterOpen();
    }

    protected override void beforeClose()
    {
        Time.timeScale = 1;
        base.beforeClose();
    }

    public void ShowMsg(string text)
    {
        m_Text.text = text;
    }

    public void AppendMsg(string text)
    {
        m_Text.text = m_Text.text + "\n" + text;
    }

    public void ResetMsg()
    {
        m_Text.text = "";
    }

    #region 作弊码输入
    void OnEndEdit(string param)
    {
        if (param != "")
        {
            bool closeWnd;
            if (CheatCodeManager.instance.ExcuteCmd(param, out closeWnd))
            {
                m_Input.text = "";
                m_OldCmd.Add(param);
                m_index = m_OldCmd.Count;
                m_Input.ActivateInputField();
                if (closeWnd)
                {
                    Close();
                }
            }
            else
            {
                m_Input.ActivateInputField();
            }
        }
    }

    void OnValueChanged(string param)
    {
        if (param.IndexOf('`') >= 0)
        {
            m_Input.text = "";
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (m_index > 0)
            {
                m_index--;
                m_Input.text = m_OldCmd[m_index];
                m_Input.MoveTextEnd(false);
            }
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (m_index < m_OldCmd.Count - 1)
            {
                m_index++;
                m_Input.text = m_OldCmd[m_index];
                m_Input.MoveTextEnd(false);
            }
        }
    }
    #endregion

}
