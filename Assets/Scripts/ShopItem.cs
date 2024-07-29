using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public RectTransform Rect { private set; get; }
    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
    }
}
