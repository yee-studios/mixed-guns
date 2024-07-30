using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplayFPS : MonoBehaviour
{
    [SerializeField] string displayText = "{0} FPS";
    [SerializeField] float updateRate = 1f;
    float nextUpdate = 0f;
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float now = Time.realtimeSinceStartup;
        if (now < nextUpdate) return;
        nextUpdate = now + updateRate;
        int fps = Mathf.RoundToInt(1f/Time.deltaTime);
        text.text = string.Format(displayText, fps);
    }
}
