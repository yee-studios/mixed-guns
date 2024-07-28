using UnityEngine;
using UnityEngine.UI;

public class HealthBorders : Singleton<HealthBorders>
{
    public Image Image { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Image = GetComponent<Image>();
    }

    private void Update()
    {
        Entity e = PlayerController.Instance?.Entity;
        if (!e) return;
        Color color = Image.color;
        color.a = 1f-(e.Health / e.MaxHealth);
        Image.color = color;
    }
}
