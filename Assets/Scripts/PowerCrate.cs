using System;
using UnityEngine;
using Random = UnityEngine.Random;

enum PowerCrateType {
    Random,
    FullVision,
    DoubleSpeed
}

public class PowerCrate : MonoBehaviour
{
    public int fullVisionTime = 30;
    public int doubleSpeedTime = 30;
    
    [SerializeField] PowerCrateType powerCrateType = PowerCrateType.Random;
    private SpriteRenderer fillingRenderer;

    private void Awake()
    {
        if (powerCrateType != PowerCrateType.Random) return;
        
        Array values = Enum.GetValues(typeof(PowerCrateType));
        powerCrateType = (PowerCrateType) values.GetValue(Random.Range(1, values.Length));
        fillingRenderer = transform.Find("Filling").GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        switch (powerCrateType)
        {
            case PowerCrateType.FullVision:
                fillingRenderer.color = Color.yellow;
                break;
            
            case PowerCrateType.DoubleSpeed:
                fillingRenderer.color = Color.blue;
                break;
        }
    }

    private void OnDestroy()
    {
        PlayerController player = PlayerController.Instance;
        if (!player) return;
        switch (powerCrateType)
        {
            case PowerCrateType.FullVision:
                player.fullVisionTimeRemaining = fullVisionTime;
                ScreenAnnouncements.SpawnAnnouncement("FULL VISION!1!!");
                break;
            
            case PowerCrateType.DoubleSpeed:
                player.speedBoostTimeRemaining = doubleSpeedTime;
                ScreenAnnouncements.SpawnAnnouncement("gotta go faaast!");
                break;
        }
    }
}
