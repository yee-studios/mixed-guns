using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SmallText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    public string text = "Small text!";
    public float t = 3f;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        _text.text = text;
        transform.DOLocalMoveY(transform.position.y+1f, t);
        _text.DOFade(0f, t);
        Destroy(gameObject, t);
    }

    internal static void Appear(Vector3 position, string text)
    {
        Instantiate(PrefabHolder.Instance.SmallText, position, Quaternion.identity).text = text;
    }
}
