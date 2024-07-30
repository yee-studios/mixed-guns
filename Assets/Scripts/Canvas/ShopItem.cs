using System;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class ShopItem : MonoBehaviour
{
    public ShopItemInfo info;
    public RectTransform Rect { private set; get; }
    public UnityEvent onSelect;
    [field: SerializeField] public Button selectButton { private set; get; }
    [field: SerializeField] public TextMeshProUGUI title { private set; get; }
    [field: SerializeField] public TextMeshProUGUI description { private set; get; }
    [field: SerializeField] public TextMeshProUGUI buttonText { private set; get; }
    internal ButtonHoverEvents hoverEvents;
    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        selectButton.onClick.AddListener(() => onSelect?.Invoke());
        hoverEvents = selectButton.GetComponent<ButtonHoverEvents>();
        buttonText = selectButton.GetComponentInChildren<TextMeshProUGUI>();
        hoverEvents.onPointerEnter.AddListener(() => buttonText.text = "Buy");
        hoverEvents.onPointerExit.AddListener(() => buttonText.text = GetButtonText());
    }

    public string GetButtonText() => info.cost == 0 ? "Free" : info.cost < 0 ? "" : info.cost.ToString();

    internal void UpdateInfo()
    {
        title.text = info.title;
        description.text = info.description;
        if (info.cost < 0)
        {
            hoverEvents.onPointerEnter.RemoveAllListeners();
            hoverEvents.onPointerExit.RemoveAllListeners();
            selectButton.interactable = false;
            return;
        }
    }
}