using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

enum PowerCrateType {
    Random,
    FullVision,
    SpeedBoost
}

public class PowerCrate : MonoBehaviour
{
    public int fullVisionTime = 30;
    public int speedBoostTime = 30;
    
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
            
            case PowerCrateType.SpeedBoost:
                fillingRenderer.color = Color.blue;
                break;
        }
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        PlayerController player = PlayerController.Instance;
        if (!player) return;
        switch (powerCrateType)
        {
            case PowerCrateType.FullVision:
                player.fullVisionTimeRemaining = fullVisionTime;
                SmallText.Appear(transform.position, $"+{fullVisionTime}s Full Vision!", Color.yellow);
                break;
            
            case PowerCrateType.SpeedBoost:
                player.speedBoostTimeRemaining = speedBoostTime;
                SmallText.Appear(transform.position, $"+{speedBoostTime}s Speed Boost!", Color.cyan);
                break;
        }
    }
}
