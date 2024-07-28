using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Entity entity;
    public Entity Entity => entity;
    AudioSource audioSource;
    private void Awake()
    {
        entity = GetComponent<Entity>();
        entity.OnDied.AddListener(OnDied);
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GetComponent<AIDestinationSetter>().target = PlayerController.Instance?.transform;
        hitRadius = GetComponent<CircleCollider2D>().radius*2;
    }

    float nextHit = 0f;
    float hitRadius = 2f;
    [SerializeField] float hitRate = 0.5f;
    [SerializeField] float hitDamage = 10f;
    
    private void Update()
    {
        float now = Time.time;
        if (now < nextHit) return;
        if (!PlayerController.Instance) return;
        if (Vector3.Distance(transform.position, PlayerController.Instance.LastPosition) > hitRadius) return;
        nextHit = now + hitRate;
        PlayerController.Instance.Entity.Health -= hitDamage;
        audioSource.PlayOneShot(AudioClipsManager.Instance.Hit);
    }

    void OnDied()
    {
        Instantiate(PrefabHolder.Instance.DeathParticles, transform.position, Quaternion.identity);
        OneShotSoundsCreator.PlayOneShotAtPosition(transform.position, AudioClipsManager.Instance.EnemyDeath, Random.Range(0.9f, 1.1f));
        CoinsManager.Instance.Coins += 10;
    }
}
