using Pathfinding;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum EnemyType { Fire, Ice }

public class Enemy : MonoBehaviour
{
    Entity entity;
    public Entity Entity => entity;
    AudioSource audioSource;
    public EnemyType type;
    [SerializeField] SpriteRenderer fill;
    private void Awake()
    {
        Array values = Enum.GetValues(typeof(EnemyType));
        type = (EnemyType)values.GetValue(Random.Range(0, values.Length));
        fill.color = type == EnemyType.Fire ? Color.red : Color.cyan;
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
        SmallText.Appear(transform.position, "+10 coins!", Color.yellow);
    }

    internal void SlowDown()
    {
        if (slowingDown) return;
        StartCoroutine(SlowDownCoroutine());
    }

    bool slowingDown = false;

    IEnumerator SlowDownCoroutine()
    {
        slowingDown = true;
        yield return new WaitForSeconds(1f);
        slowingDown = false;
    }
}
