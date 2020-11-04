using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UICombatMainPlayerItem : MonoBehaviour, IUIComponent, IPointerClickHandler
{
    public void Initialize()
    {

    }

    EAvatar m_Avatar;

    public void SetPlayer(EAvatar avatar)
    {
        m_Avatar = avatar;
        gameObject.SetActive(true);
    }

    public void ResetPlayer()
    {
        m_Avatar = null;
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogError("Click");
    }
}
