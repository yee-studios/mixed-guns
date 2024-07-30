using UnityEngine;

public enum BulletType
{
    Normal,
    Explosive
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
        
        if (type == BulletType.Explosive) Explode();

        Destroy(gameObject);

        Vector3 point = collision.ClosestPoint(transform.position);
        OneShotSoundsCreator.Instance?.BulletImpact(point);
        if (collision.CompareTag("Enemy")) Instantiate(PrefabHolder.Instance.BloodParticles, point, Quaternion.FromToRotation(collision.transform.position, transform.position));
        Entity e = collision.GetComponentInParent<Entity>();

        if (!e) return;
        e.Health -= Random.Range(gun.MinDamage, gun.MaxDamage);
    }

    void Explode()
    {
        Instantiate(PrefabHolder.Instance.Explosion, transform.position, Quaternion.identity);
        foreach (var col in Physics2D.OverlapCircleAll(transform.position, explodeRadius))
        {
            Entity e = col.GetComponent<Entity>();
            if (!e) continue;
            e.Health -= explodeDamage;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (type == BulletType.Explosive)
            Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
    
}
