using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;
using UnityEditorInternal.Profiling.Memory.Experimental;
using Random = UnityEngine.Random;

[Serializable]
public struct ShopItemInfo
{
    [field: SerializeField] public string id { set; get; }
    [field: SerializeField] public string title { set; get; }
    [field: SerializeField] public string description { set; get; }
    [field: SerializeField] public int cost { set; get; }
}

public class ShopController : Singleton<ShopController>
{
    [SerializeField] float offset = 10f;
    [SerializeField] RectTransform itemsContainer;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] int itemsCount = 10;
    [SerializeField] Button buyButton;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] List<ShopItemInfo> items = new();
    [SerializeField] List<ShopItem> shopItems = new();

    PlayerInput input;
    public bool shopOpen {private set; get;}

    protected override void Awake()
    {
        base.Awake();
        transform.localScale = Vector3.one;
        for (int i = 0; i < items.Count; i++)
        {
            ShopItem newItem = Instantiate(PrefabHolder.Instance.ShopItemPrefab, itemsContainer);
            shopItems.Add(newItem);
            newItem.Rect.anchoredPosition = new Vector3(0, -i * newItem.Rect.rect.height - offset * i, 0);
            itemsContainer.sizeDelta = new Vector2(0, itemsCount * newItem.Rect.rect.height + offset * itemsCount);

            ShopItemInfo info = items[i];
            newItem.info = info;
            newItem.UpdateInfo();

            newItem.onSelect.AddListener(() => Selected(newItem));
        }

        UpdateItemButtons();
        titleText.rectTransform.anchoredPosition = new Vector3(0, 50f, 0);
        buyButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -50f, 0);
        scrollRect.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
    }

    void Selected(ShopItem item)
    {
        PlayerController player = PlayerController.Instance;
        switch (item.info.id) {
            case "firemode":
                if(player.Gun.FireMode == FireMode.Semi)
                {
                    player.Gun.FireMode = FireMode.Burst;
                    item.info.cost = 50;
                    item.UpdateInfo();
                    ScreenAnnouncements.SpawnAnnouncement("Burst fire mode!");
                } else if (player.Gun.FireMode == FireMode.Burst)
                {
                    player.Gun.FireMode = FireMode.Auto;
                    item.info.cost = -1;
                    item.UpdateInfo();
                    ScreenAnnouncements.SpawnAnnouncement("Automatic fire mode!");
                }
                break;
            case "dashes":
                player.maxDashes++;
                ScreenAnnouncements.SpawnAnnouncement($"Max dashes are now {player.maxDashes}");
                item.info.cost *= 2;
                break;
            default:
                break;
        }
        UpdateItemButtons();
    }

    void UpdateItemButtons() {
        foreach(ShopItem item in shopItems)
        {
            int cost = item.info.cost;
            /*
            if (cost > 0) item.buttonText.text = cost.ToString();
            else if (cost == 0) item.buttonText.text = "Free";
            else item.buttonText.text = "";
            */
            item.buttonText.text = item.GetButtonText();
            item.selectButton.interactable = cost >= 0 ? CoinsManager.Instance.Coins >= cost : false;
        }
    }

    private void Start()
    {
        input = FindObjectOfType<PlayerInput>();
    }

    private void Update()
    {
        PlayerController player = PlayerController.Instance;
        if (shopOpen && !player) ToggleShop();
        if(!player || player.startAnimation || !input.actions["shop"].WasPressedThisFrame()) return;
        ToggleShop();
    }

    float shopTime = 0.25f;
    void ToggleShop()
    {
        UpdateItemButtons();
        shopOpen = !shopOpen;
        itemsContainer.anchoredPosition = Vector3.zero;
        MusicController.Instance.ShopMusic(shopOpen);
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, shopOpen ? 0f : 1f, shopTime).SetUpdate(true);
        DOTween.To(() => titleText.rectTransform.anchoredPosition,
            x => titleText.rectTransform.anchoredPosition = x, shopOpen ? Vector2.zero : new Vector3(0, 50f, 0), shopTime).SetUpdate(true);
        DOTween.To(() => buyButton.GetComponent<RectTransform>().anchoredPosition,
            x => buyButton.GetComponent<RectTransform>().anchoredPosition = x,
            shopOpen ? new Vector2(0, -10f) : new Vector2(0, -50f), shopTime).SetUpdate(true);
        if (shopOpen) scrollRect.gameObject.SetActive(true);
        scrollRect.transform.DOScaleY(shopOpen ? 1f : 0f, shopTime).ChangeStartValue(shopOpen ? Vector3.zero : Vector3.one).SetUpdate(true)
            .OnComplete(() => { if (!shopOpen) scrollRect.gameObject.SetActive(false); });
    }
}
