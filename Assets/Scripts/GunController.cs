using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class GunController : MonoBehaviour
{
    [SerializeField] AudioClip[] shootSounds;
    [SerializeField] Transform tip;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] float bulletSpeed = 1000f;
    [SerializeField] FireMode fireMode = FireMode.Semi;
    public bool triggerStatus = false;
    AudioSource audioSource;

    [SerializeField] float fireRate = 0.25f;
    [SerializeField] int burstAmount = 3;
    int currentBurst = 0;

    float nextFire = 0f;


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

    public bool UpdateTrigger(bool triggerStatus)
    {
        if (this.triggerStatus == triggerStatus) return false;
        this.triggerStatus = triggerStatus;
        if(triggerStatus && fireMode == FireMode.Semi) Shoot();
        return true;
    }

    private void Update()
    {
        if (fireMode == FireMode.Semi
            || !triggerStatus
            || Time.time <= nextFire) return;
        Shoot();
    }

    private void Shoot()
    {
        PlayerController.Instance.surroundingLight.pointLightOuterRadius += 1f;
        audioSource.PlayOneShot(shootSounds[Random.Range(0, shootSounds.Length-1)]);
        Bullet b = Instantiate(bulletPrefab, tip.position, tip.rotation);
        b.speed = bulletSpeed;
        if(fireMode == FireMode.Burst)
        {
            currentBurst++;
            if (currentBurst >= burstAmount)
            {
                nextFire = Time.time + (fireRate * burstAmount);
                currentBurst = 0;
                return;
            }
        }
        nextFire = Time.time + fireRate;
    }
}

enum FireMode { Semi, Burst, Auto }