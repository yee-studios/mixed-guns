using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        DeathScreen.Instance.BreatheIn();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DeathScreen.Instance.BreatheOut();
    }
}
