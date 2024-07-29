using System.Collections;
using System.Collections.Generic;
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

    [field: Header("UI")]
    [field: SerializeField] public ShopItem ShopItemPrefab { get; private set; }
}
