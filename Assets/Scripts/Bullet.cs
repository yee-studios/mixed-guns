using UnityEngine;

public enum BulletType
{
    Normal,
    Explosive,
    Freezing,
}

public class Bullet : MonoBehaviour
{
    [Header("Bullet Properties")]
    public GunController gun;
    Rigidbody2D rb;
    public BulletType type;
    public float speed = 1000f;
    [SerializeField] float destroyTime = 10f;
    
    [Header("Explosive")]
    [SerializeField] private GameObject explodeEffect;
    [SerializeField] float explodeDamage = 25f;
    [SerializeField] float explodeRadius = 3f;
    
    bool hit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.AddForce(transform.up * speed);
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hit) return;
        hit = true;

        Destroy(gameObject);

        Vector3 point = collision.ClosestPoint(transform.position);
        OneShotSoundsCreator.Instance?.BulletImpact(point);
        if (collision.CompareTag("Enemy")) Instantiate(PrefabHolder.Instance.BloodParticles, point, Quaternion.FromToRotation(collision.transform.position, transform.position));
        Entity e = collision.GetComponentInParent<Entity>();

        if (!e) return;
        float dmg = Random.Range(gun.MinDamage, gun.MaxDamage);
        e.Health -= dmg;
        if (e.Health <= 0 && type != BulletType.Normal) Explode();
    }

    void Explode()
    {
        Entity pe = PlayerController.Instance.Entity;
        Instantiate(type == BulletType.Explosive ? PrefabHolder.Instance.Explosion : PrefabHolder.Instance.FreezingParticles,
            transform.position, Quaternion.identity);
        foreach (var col in Physics2D.OverlapCircleAll(transform.position, explodeRadius))
        {
            Entity e = col.GetComponent<Entity>();
            if (!e || e == pe) continue;
            e.Health -= explodeDamage;
            if (type == BulletType.Freezing && e.TryGetComponent(out Enemy enemy)
                && enemy.type != EnemyType.Ice) enemy.SlowDown();
        }
    }
    
    private void OnDrawGizmos()
    {
        if (type == BulletType.Explosive)
            Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
    
}
