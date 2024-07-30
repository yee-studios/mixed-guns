using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class GunController : MonoBehaviour
{
    [SerializeField] Transform tip;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] float bulletSpeed = 1000f;
    [SerializeField] FireMode fireMode = FireMode.Semi;
    [SerializeField] BulletType bulletType = BulletType.Normal;
    public bool triggerStatus = false;
    AudioSource audioSource;

    [SerializeField] float fireRate = 0.25f;
    [SerializeField] int burstAmount = 3;
    [SerializeField] float rotationSmooth = 10f;
    [SerializeField] float lightExpansionWhenShooting = 1f;
    int currentBurst = 0;
    float nextFire = 0f;

    [SerializeField] float minDamage = 10f;
    public float MinDamage => minDamage;
    [SerializeField] float maxDamage = 20f;
    public float MaxDamage => maxDamage;

    private AudioClip[] shootSounds;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        shootSounds = AudioClipsManager.Instance.Shoot;
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
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * rotationSmooth);
    }

    public void SwitchFireMode()
    {
        audioSource.PlayOneShot(AudioClipsManager.Instance.FireModeSwitch);
        int f = (int)fireMode;
        f++;
        fireMode = (FireMode)f;
        if (!Enum.IsDefined(typeof(FireMode), fireMode)) fireMode = FireMode.Semi;
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
        float pointLightOuterRadius = PlayerController.Instance.surroundingLight.pointLightOuterRadius;
        DOTween.To(() => PlayerController.Instance.surroundingLight.pointLightOuterRadius,
            x => PlayerController.Instance.surroundingLight.pointLightOuterRadius = x,
            pointLightOuterRadius += lightExpansionWhenShooting, 1);
        audioSource.PlayOneShot(shootSounds[Random.Range(0, shootSounds.Length-1)]);
        Bullet b = Instantiate(bulletPrefab, tip.position, tip.rotation);
        b.speed = bulletSpeed;
        b.gun = this;
        b.type = bulletType;
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