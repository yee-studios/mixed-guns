using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeUIUnit : MonoBehaviour
{
    [SerializeField] RectTransform background;
    [SerializeField] RectTransform indicator;

    public void UpdateIndicator(float i) => indicator.localScale = new Vector3(1, Mathf.Clamp01(i), 1);
}
