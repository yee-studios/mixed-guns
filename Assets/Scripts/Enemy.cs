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
    float normalMaxSpeed = 3f;
    float slowDownSpeed = 1f;
    AIPath aipath;
    private void Awake()
    {
        aipath = GetComponent<AIPath>();
        normalMaxSpeed = aipath.maxSpeed;
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
        PlayerController player = PlayerController.Instance;
        if (!player) return;
        if (Vector3.Distance(transform.position, player.LastPosition) > hitRadius) return;
        nextHit = now + hitRate;
        player.Entity.Health -= hitDamage;
        audioSource.PlayOneShot(AudioClipsManager.Instance.Hit);
    }

    void OnDied()
    {
        Instantiate(PrefabHolder.Instance.DeathParticles, transform.position, Quaternion.identity);
        OneShotSoundsCreator.PlayOneShotAtPosition(transform.position, AudioClipsManager.Instance.EnemyDeath, Random.Range(0.9f, 1.1f));
        CoinsManager.Instance.Coins += 10;
        SmallText.Appear(transform.position, "+10 coins!", Color.yellow);
        if(PlayerController.Instance) PlayerController.Instance.kills++;
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
        aipath.maxSpeed = slowDownSpeed;
        yield return new WaitForSeconds(1f);
        aipath.maxSpeed = normalMaxSpeed;
        slowingDown = false;
    }
}
