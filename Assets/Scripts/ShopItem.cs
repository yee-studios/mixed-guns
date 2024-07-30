using System;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class ShopItem : MonoBehaviour
{
    ShopItemInfo info;
    public ShopItemInfo Info { get { return info; } set {
            info = value;
            cost = info.cost;
            title.text = info.title;
            description.text = info.description;
        } }
    public RectTransform Rect { private set; get; }
    public UnityEvent onSelect;
    [field: SerializeField] public Button selectButton { private set; get; }
    [field: SerializeField] public TextMeshProUGUI title { private set; get; }
    [field: SerializeField] public TextMeshProUGUI description { private set; get; }
    [field: SerializeField] public TextMeshProUGUI buttonText { private set; get; }
    internal ButtonHoverEvents hoverEvents;
    public int cost = 0;
    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
        selectButton.onClick.AddListener(() => onSelect?.Invoke());
        hoverEvents = selectButton.GetComponent<ButtonHoverEvents>();
        buttonText = selectButton.GetComponentInChildren<TextMeshProUGUI>();
        hoverEvents.onPointerEnter.AddListener(() => buttonText.text = "Buy");
        hoverEvents.onPointerExit.AddListener(() => buttonText.text = GetButtonText());
    }

    public string GetButtonText() => cost == 0 ? "Free" : cost < 0 ? "" : cost.ToString();

    internal void UpdateInfo(int cost)
    {
        this.cost = cost;
        if (cost < 0)
        {
            hoverEvents.onPointerEnter.RemoveAllListeners();
            hoverEvents.onPointerExit.RemoveAllListeners();
            selectButton.interactable = false;
            return;
        }
    }
}