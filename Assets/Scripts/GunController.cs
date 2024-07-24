using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunController : MonoBehaviour
{
    [SerializeField] AudioClip[] shootSounds;
    [SerializeField] Transform tip;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] float bulletSpeed = 1000f;
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        audioSource.PlayOneShot(shootSounds[Random.Range(0, shootSounds.Length-1)]);
        Bullet b = Instantiate(bulletPrefab, tip.position, tip.rotation);
        b.speed = bulletSpeed;
    }
}
