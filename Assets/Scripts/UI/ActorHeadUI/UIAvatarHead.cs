using UnityEngine;
using UnityEngine.UI;

public class UIAvatarHead : UIFollowEntity
{
    public static UIAvatarHead Create()
    {
        GameObject prefab = Resources.Load<GameObject>("UI/HeadUI/UIAvatarHead");
        GameObject obj = GameObject.Instantiate<GameObject>(prefab);
        obj.name = prefab.name;
        UIAvatarHead ui = obj.GetComponent<UIAvatarHead>();
        ui.Initialize();
        return ui;
    }

    Slider m_Hp;
    Text m_HpText;

    GameObject m_CurTurn;

    EAvatar m_Avatar;

    protected override void OnInitialize()
    {
        m_Hp = transform.Find("Hp").GetComponent<Slider>();
        m_HpText = m_Hp.transform.Find("Text").GetComponent<Text>();
        m_CurTurn = transform.Find("CurTurn").gameObject;
    }

    public void SetAvatar(EAvatar avatar)
    {
        ResetAvatar();
        m_Avatar = avatar;
        avatar.controllerComponent.onTurnStart.AddListener(OnTurnStart);
        avatar.controllerComponent.onTurnComplete.AddListener(OnTurnComplete);
        m_Avatar.propertyComponent.onHpChanged.AddListener(UpdateHp);
        UpdateHp(m_Avatar.propertyComponent);
        if (TurnBaseManager.instance.curTurnBase != null && TurnBaseManager.instance.curTurnBase.curController == avatar.controllerComponent)
        {
            m_CurTurn.gameObject.SetActive(true);
        }
        else
        {
            m_CurTurn.gameObject.SetActive(false);
        }
    }

    public void ResetAvatar()
    {
        if (m_Avatar != null)
        {
            m_Avatar.controllerComponent.onTurnStart.RemoveListener(OnTurnStart);
            m_Avatar.controllerComponent.onTurnComplete.RemoveListener(OnTurnComplete);
            m_Avatar.propertyComponent.onHpChanged.RemoveListener(UpdateHp);
            m_Avatar = null;
        }
    }

    public override void ResetData()
    {
        ResetAvatar();
        base.ResetData();
    }

    void UpdateHp(CPropertyComponent propertyComponent)
    {
        if (propertyComponent != null && propertyComponent.hpMax > 0f)
        {
            m_Hp.value = propertyComponent.hp / propertyComponent.hpMax;
            m_HpText.text = Untility.Tool.FloatToString(propertyComponent.hp);
        }
    }

    void OnTurnStart(CControllerComponent t, TurnBase turnBase)
    {
        m_CurTurn.gameObject.SetActive(true);
    }

    void OnTurnComplete(CControllerComponent t, TurnBase turnBase)
    {
        m_CurTurn.gameObject.SetActive(false);
    }
}
