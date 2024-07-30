using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] public Entity target;
    [SerializeField] Vector3 offset;
    [SerializeField] RectTransform fill;
    [SerializeField] float lerpSpeed = 10f;
    [SerializeField] float lerpSpeed2 = 10f;
    [SerializeField] float value = 1f;

    Vector3 pos => target.transform.position + offset;

    private void Start()
    {
        transform.position = pos;
    }

    private void Update()
    {
        fill.localScale = new Vector3(Mathf.Clamp01(Mathf.Lerp(fill.localScale.x, value, lerpSpeed2 * Time.deltaTime)), 1, 1);
        fill.anchoredPosition = new Vector3(Mathf.Clamp01(Mathf.Lerp(fill.anchoredPosition.x, value * .5f, lerpSpeed2 * Time.deltaTime)), 0, 0);
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, pos, lerpSpeed * Time.deltaTime);
    }

    public void UpdateValue(float value)
    {
        this.value = value;
    }
}
