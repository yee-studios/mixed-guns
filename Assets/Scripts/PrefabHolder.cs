using UnityEngine;

public class PrefabHolder : PersistentSingleton<PrefabHolder>
{
    [field: Header("Prefabs")]
    [field: SerializeField] public HealthBar HealthBarPrefab { get; private set; }
    [field: SerializeField] public Enemy EnemyPrefab { get; private set; }

    [field: Header("Particles")]
    [field: SerializeField] public ParticleSystem FartParticles { get; private set; }
    [field: SerializeField] public ParticleSystem BloodParticles { get; private set; }
    [field: SerializeField] public ParticleSystem DeathParticles { get; private set; }

    [Tooltip("This prefab also contains an AudioSource that plays an explosion sound.")]
    [field: SerializeField] public ParticleSystem Explosion { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public ShopItem ShopItemPrefab { get; private set; }
    [field: SerializeField] public ScreenAnnouncement ScreenAnnouncementPrefab { get; private set; }
    [field: SerializeField] public SmallText SmallText { get; private set; }
}
