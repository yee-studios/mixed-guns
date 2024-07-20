using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUIUnit : MonoBehaviour
{
    [SerializeField] RectTransform background;
    [SerializeField] Image indicator;

    public void UpdateIndicator(float i) => indicator.fillAmount = i;
}
