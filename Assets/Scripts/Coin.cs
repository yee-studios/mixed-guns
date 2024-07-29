using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    Transform body;
    Rigidbody2D rb;
    AudioSource audioSource;
    bool grabbed = false;
    float r = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        body = transform.GetChild(0);
    }

    private void Update()
    {
        if (grabbed) return;
        body.localPosition = new Vector3(0,Mathf.Sin(Time.time*3f),0)*0.1f;
        body.rotation = Quaternion.Euler(Mathf.Sin(Time.time) * 45f, Mathf.Cos(Time.time) * 45f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (grabbed) return;
        if (!collision.transform.GetComponentInParent<PlayerController>()) return;
        grabbed = true;
        audioSource.Play();
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddForce(new Vector2(Random.Range(-r, r), Random.Range(-r, r))+Vector2.up*r, ForceMode2D.Impulse);
        PlayerController.Instance.Entity.Health += Random.Range(5f, 15f);
        CoinsManager.Instance.Coins += 20;
        Destroy(gameObject, 5f);
    }
}
