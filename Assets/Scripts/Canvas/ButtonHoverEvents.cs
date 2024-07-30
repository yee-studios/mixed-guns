using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonHoverEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField] public UnityEvent onPointerEnter { get; private set; }
    [field: SerializeField] public UnityEvent onPointerExit {  get; private set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit.Invoke();
    }
}
