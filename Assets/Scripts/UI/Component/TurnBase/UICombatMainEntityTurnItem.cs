using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICombatMainEntityTurnItem : MonoBehaviour, IUIComponent
{
    Text m_Text;
    Image m_Image;
    public void Initialize()
    {
        m_Text = transform.Find("Text").GetComponent<Text>();
        m_Image = GetComponent<Image>();
    }


    public void SetController(CControllerComponent t)
    {
        m_Text.text = t.entity.keyName;
        Relation relation = CTeamComponent.GetRelation(TeamId.Team_0, t.teamId);
        switch (relation)
        {
            case Relation.Enemy:
                m_Image.color = Color.red;
                break;
            case Relation.Teammate:
                m_Image.color = Color.green;
                break;
        }
        gameObject.SetActive(true);
    }

    public void ResetController()
    {
        gameObject.SetActive(false);
        transform.SetAsLastSibling();
    }
}
