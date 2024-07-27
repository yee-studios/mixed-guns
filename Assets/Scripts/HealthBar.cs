using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] public Entity target;
    [SerializeField] Vector3 offset;
    [SerializeField] RectTransform fill;
    [SerializeField] float lerpSpeed = 10f;

    Vector3 pos => target.transform.position + offset;

    private void Start()
    {
        transform.position = pos;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, pos, lerpSpeed * Time.deltaTime);
    }

    public void UpdateValue(float value)
    {
        fill.localScale = new Vector3(Mathf.Clamp01(value),1,1);
        fill.anchoredPosition = new Vector3(Mathf.Clamp01(value*.5f),0,0);
    }
}
