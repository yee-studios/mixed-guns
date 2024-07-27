using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabHolder : PersistentSingleton<PrefabHolder>
{
    [field: SerializeField] public HealthBar HealthBarPrefab { get; private set; }
    [field: SerializeField] public Enemy EnemyPrefab { get; private set; }
}
