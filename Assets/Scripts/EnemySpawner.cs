using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] float spawnRate = 1f;
    [SerializeField] int maxEnemies = 5;
    [SerializeField] List<Enemy> enemies;
    [SerializeField] float r = 50f;
    [SerializeField] float lastSpawn = 0f;
    [SerializeField] bool spawning = true;

    private void Update()
    {
        if (!spawning) return;
        if (enemies.Count >= maxEnemies) return;
        float now = Time.time;
        if (now < lastSpawn + spawnRate) return;
        lastSpawn = now;
        Vector3 randomPos = transform.position + new Vector3(Random.Range(-r, r), Random.Range(-r, r));
        Enemy enemy = Instantiate(enemyPrefab, randomPos.normalized * Random.Range(-r, r), Quaternion.identity);
        enemy.Entity.OnDied.AddListener(() => enemies.Remove(enemy));
        enemies.Add(enemy);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, r);
    }
}
