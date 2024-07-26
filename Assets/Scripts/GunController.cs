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

    private void LateUpdate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        
        if (Vector3.Distance(mousePos, PlayerController.Instance.transform.position) <= 1.5f) return;
        float angleRad = Mathf.Atan2(
            transform.position.x - mousePos.x,
            mousePos.y - transform.position.y);
        float angleDeg = 180 / Mathf.PI * angleRad;
        Quaternion newRot = Quaternion.Euler(0, 0, angleDeg);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 10f);
    }

    public void Shoot()
    {
        audioSource.PlayOneShot(shootSounds[Random.Range(0, shootSounds.Length-1)]);
        Bullet b = Instantiate(bulletPrefab, tip.position, tip.rotation);
        b.speed = bulletSpeed;
    }
}
