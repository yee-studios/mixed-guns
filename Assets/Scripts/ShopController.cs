using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;

public class ShopController : Singleton<ShopController>
{
    [SerializeField] float offset = 10f;
    [SerializeField] RectTransform itemsContainer;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] int itemsCount = 10;
    [SerializeField] Button buyButton;
    [SerializeField] TextMeshProUGUI titleText;

    PlayerInput input;
    public bool shopOpen {private set; get;}

    protected override void Awake()
    {
        base.Awake();
        transform.localScale = Vector3.one;
        for (int i = 0; i < itemsCount; i++)
        {
            ShopItem newItem = Instantiate(PrefabHolder.Instance.ShopItemPrefab, itemsContainer);
            newItem.Rect.anchoredPosition = new Vector3(0, -i * newItem.Rect.rect.height - offset * i, 0);
            itemsContainer.sizeDelta = new Vector2(0, itemsCount * newItem.Rect.rect.height + offset * itemsCount);
        }
        titleText.rectTransform.anchoredPosition = new Vector3(0, 50f, 0);
        buyButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -50f, 0);
        scrollRect.gameObject.SetActive(false);
    }

    private void Start()
    {
        input = FindObjectOfType<PlayerInput>();
    }

    private void Update()
    {
        if (shopOpen && !PlayerController.Instance) ToggleShop();
        if (!PlayerController.Instance || PlayerController.Instance.startAnimation ||
            !input.actions["shop"].WasPressedThisFrame()) return;
        ToggleShop();
    }

    float shopTime = 0.25f;
    void ToggleShop()
    {
        shopOpen = !shopOpen;
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
