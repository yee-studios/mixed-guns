using System.Collections.Generic;
using UnityEngine;

public class EntityMaxSpawner : MonoBehaviour
{
    [SerializeField] GameObject entityPrefab;
    [SerializeField] float spawnRate = 5f;
    [SerializeField] float spawnRateMinus = 0.25f;
    [SerializeField] int maxEntities = 1;
    [SerializeField] int maxEntitiesPlus = 1;
    [SerializeField] List<GameObject> entities;
    [SerializeField] float r = 50f;
    [SerializeField] float lastSpawn = 0f;
    [SerializeField] bool spawning = true;

    private void Update()
    {
        if (!spawning) return;
        if (entities.Count >= maxEntities) return;
        float now = Time.time;
        if (now < lastSpawn + spawnRate) return;
        lastSpawn = now;
        Vector3 randomPos = transform.position + new Vector3(Random.Range(-r, r), Random.Range(-r, r));
        GameObject entity = Instantiate(entityPrefab, randomPos.normalized * Random.Range(-r, r), Quaternion.identity);
        entity.GetComponent<Entity>().OnDied.AddListener(() =>
        {
            entities.Remove(entity);
            spawnRate -= spawnRateMinus;
            maxEntities += maxEntitiesPlus;
        });
        entities.Add(entity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, r);
    }
}
