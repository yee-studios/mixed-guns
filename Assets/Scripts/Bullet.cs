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
        Entity e = collision.GetComponentInParent<Entity>();
        if (!e) return;
        e.Health -= Random.Range(gun.MinDamage, gun.MaxDamage);
        Destroy(gameObject);
    }
}
