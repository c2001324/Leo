using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UICombatMainController : MonoBehaviour
{
    Button m_EndTurn;
    Button m_Attack;
    Slider m_Hp;
    Text m_HpText;
    Slider m_MovePoint;
    Text m_MovePointText;

    CPropertyComponent m_PropertyComponent;

    public void Initialize()
    {
        m_EndTurn = transform.Find("EndTurn").GetComponent<Button>();
        m_EndTurn.onClick.AddListener(OnClickEndTurn);

        m_Attack = transform.Find("Attack").GetComponent<Button>();
        m_Attack.onClick.AddListener(OnAttack);

        m_Hp = transform.Find("Hp").GetComponent<Slider>();
        m_HpText = m_Hp.transform.Find("Text").GetComponent<Text>();

        m_MovePoint = transform.Find("MovePoint").GetComponent<Slider>();
        m_MovePointText = m_MovePoint.transform.Find("Text").GetComponent<Text>();
    }

    public void Open()
    {
        UpdateEndTurnButtonState();
        GameEvent.TurnBaseEvent.onEntityTurnStart.AddListener(OnTurnStart);
        GameEvent.TurnBaseEvent.onEntityTurnComplete.AddListener(OnTurnComplete);
    }

    public void Close()
    {
        GameEvent.TurnBaseEvent.onEntityTurnStart.RemoveListener(OnTurnStart);
        GameEvent.TurnBaseEvent.onEntityTurnComplete.RemoveListener(OnTurnComplete);
    }

    void OnClickEndTurn()
    {
        TurnBaseManager.instance.curController.TurnComplete();
    }

    void UpdateEndTurnButtonState()
    {
        if (TurnBaseManager.instance.curController != null && TurnBaseManager.instance.curController.controller.GetType() == typeof(InputController))
        {
            m_EndTurn.gameObject.SetActive(true);
            m_Hp.gameObject.SetActive(true);
        }
        else
        {
            m_EndTurn.gameObject.SetActive(false);
            m_Hp.gameObject.SetActive(false);
        }
    }

    void OnTurnStart(CControllerComponent t)
    {
        m_PropertyComponent = t.entity.GetCComponent<CPropertyComponent>();
        m_PropertyComponent.onMovePointChanged.AddListener(OnMovePointChanged);
        m_PropertyComponent.onHpChanged.AddListener(UpdateHp);
        UpdateHp(m_PropertyComponent);
        UpdateMovePoint(m_PropertyComponent);
        UpdateEndTurnButtonState();
    }

    void OnTurnComplete(CControllerComponent t)
    {
        UpdateEndTurnButtonState();
        if (m_PropertyComponent != null)
        {
            m_PropertyComponent.onMovePointChanged.RemoveListener(OnMovePointChanged);
            m_PropertyComponent.onHpChanged.RemoveListener(UpdateHp);
            m_PropertyComponent = null;
        }
    }

    void OnAttack()
    {

    }

    void UpdateHp(CPropertyComponent propertyComponent)
    {
        if (propertyComponent != null)
        {
            m_Hp.value = propertyComponent.hp / propertyComponent.hpMax;
            m_HpText.text = Untility.Tool.FloatToString(propertyComponent.hp);
        }
    }


    void OnMovePointChanged(CPropertyComponent propertyComponent, float oldValue, float newValue)
    {
        UpdateMovePoint(propertyComponent);
    }

    void UpdateMovePoint(CPropertyComponent propertyComponent)
    {
        int movePoint = Untility.Tool.GetIntByMin(propertyComponent.movePoint);
        int movePointMax = Untility.Tool.GetIntByMin(propertyComponent.movePointMax);
        m_MovePointText.text = movePoint + " / " + movePointMax;
        m_MovePoint.value = (float)movePoint / (float)movePointMax;
    }
}
