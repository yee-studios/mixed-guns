using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public GunController gun;
    Rigidbody2D rb;
    public float speed = 1000f;
    [SerializeField] float destroyTime = 10f;
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
        Vector3 point = collision.ClosestPoint(transform.position);
        OneShotSoundsCreator.Instance?.BulletImpact(point);
        Instantiate(PrefabHolder.Instance.BloodParticles, point, Quaternion.FromToRotation(collision.transform.position, transform.position));
        Destroy(gameObject);
        Entity e = collision.GetComponentInParent<Entity>();
        if (!e) return;
        e.Health -= Random.Range(gun.MinDamage, gun.MaxDamage);
    }
}
