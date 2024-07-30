using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] Transform prefab;
    [SerializeField] float spawnRate = 5f;
    [SerializeField] float r = 10f;
    float nextSpawn = 0f;

    private void Update()
    {
        if (Time.time < nextSpawn) return;
        nextSpawn = Time.time + spawnRate;
        Instantiate(prefab, new Vector2(Random.Range(-r, r), Random.Range(-r, r)), Quaternion.identity, transform);
    }
}
