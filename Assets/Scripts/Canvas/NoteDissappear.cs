using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoteDissappear : MonoBehaviour
{
    TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.DOFade(0f, 3f).SetDelay(2f);
        Destroy(gameObject, 5f);
    }
}
