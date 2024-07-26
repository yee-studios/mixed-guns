using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] Transform fill;
    [SerializeField] float lerpSpeed = 10f;

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, lerpSpeed * Time.deltaTime);
    }

    public void UpdateValue(float value)
    {
        fill.localScale = new Vector3(Mathf.Clamp01(value),1,1);
    }
}
