using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Singleton<PlayerController>
{
    #region Cached components
    Rigidbody2D rb;
    PlayerInput input;
    AudioSource audioSource;
    Entity entity;
    public Entity Entity => entity;
    Vector2 lastMove = new();
    public Vector2 LastPosition { get; private set; }
    #endregion

    #region Unity fields
    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float dashForce = 1000f;
    [SerializeField] int availableDashes = 0;

    int maxDashes;
    public int MaxDashes
    {
        get { return maxDashes; }
        set
        {
            maxDashes = value;
            DashChargeUIController.Instance?.UpdateUI();
        }
    }
    [SerializeField] float dashReloadTime = 1f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] float currentDashReload = 0f;

    [Header("Lightning")]
    [Range(0f, 1f)]
    [SerializeField] float lightReductionRate = 0.25f;
    [SerializeField] float minLightOuter = 3f;
    [SerializeField] float maxLightOuter = 8f;
    public Light2D flashLight;
    public Light2D surroundingLight;

    [Header("Camera")]
    [Range(0f, 5f)]
    [SerializeField] float screenShakeIntensity = 1.5f;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    CinemachineBasicMultiChannelPerlin camNoise;
    [SerializeField] float cameraMinSize = 4f;
    [SerializeField] float cameraMaxSize = 5f;

    [Header("Powerups")]
    public float speedBoostMultiplier = 1.5f;
    public int speedBoostTimeRemaining;
    public int fullVisionTimeRemaining;

    public List<BulletType> availableBulletTypes = new();

    public bool startAnimation { private set; get; }

    [Header("Audio")]
    [SerializeField] float constantSoundVolume = 0.25f;
    [SerializeField] AudioSource movingAudio;
    [SerializeField] AudioSource rotatingAudio;

    [SerializeField] ParticleSystem fart;

    [field: Header("Other")]
    [field: SerializeField] public GunController Gun { get; private set; }

    [SerializeField] float debugDeathTime = 0f;
    public int kills = 0;
    #endregion

    bool speedBoost;
    bool fullVision;
    Tween fullVisionTweenInProgress;
    float timeSinceLastDash;
    Image dashCooldownFill;

    #region Unity methods
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        entity = GetComponent<Entity>();
        entity.OnDied.AddListener(OnDied);
        camNoise = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        StartCoroutine(EverySecond());
    }

    readonly float delay1 = 3f;
    readonly float delay2 = .25f;

    protected void Start()
    {
        input = FindObjectOfType<PlayerInput>();
        dashCooldownFill = DashChargeUIController.Instance.cooldownFill;
        startAnimation = true;
        entity.invencibility = true;
        //EnemySpawner.Instance.enabled = false;
        DOTween.To(() => surroundingLight.falloffIntensity, x => surroundingLight.falloffIntensity = x, 0.5f, delay1)
            .ChangeStartValue(1f).SetDelay(delay2)
            .OnComplete(() =>
            {
                entity.invencibility = false;
                startAnimation = false;
                ScreenAnnouncements.SpawnAnnouncement(
                    $"Press {input.actions["shop"].GetBindingDisplayString()}" +
                    "\nto open the shop and buy things!");
                //EnemySpawner.Instance.enabled = true;
            });

        DOTween.To(() => minLightOuter, x => minLightOuter = x, 3f, delay1).ChangeStartValue(0f).SetDelay(delay2);
        DOTween.To(() => surroundingLight.pointLightOuterRadius, x => surroundingLight.pointLightOuterRadius = x,
            surroundingLight.pointLightOuterRadius, delay1).ChangeStartValue(0f).SetDelay(delay2).SetUpdate(UpdateType.Late);

        DOTween.To(() => flashLight.falloffIntensity, x => flashLight.falloffIntensity = x, 0.5f, delay1).ChangeStartValue(1f).SetDelay(delay2);
        DOTween.To(() => flashLight.pointLightInnerRadius, x => flashLight.pointLightInnerRadius = x,
            flashLight.pointLightInnerRadius, delay1).ChangeStartValue(0f).SetDelay(delay2);
        DOTween.To(() => flashLight.pointLightOuterRadius, x => flashLight.pointLightOuterRadius = x,
            flashLight.pointLightOuterRadius, delay1).ChangeStartValue(0f).SetDelay(delay2);
    }

    void OnDied()
    {
        OneShotSoundsCreator.PlayOneShotAtPosition(transform.position, AudioClipsManager.Instance.PlayerDeath);
        DeathScreen.Instance.Initialize();
        MusicController.Instance.DeathMusic();
        DOTween.To(() => surroundingLight.falloffIntensity, x => surroundingLight.falloffIntensity = x, 1f, 1f);
        DOTween.To(() => surroundingLight.pointLightOuterRadius, x => surroundingLight.pointLightOuterRadius = x, 0f, 1f);
    }

    private void Update()
    {
        if (ShopController.Instance.shopOpen) return;

#if UNITY_EDITOR
        if (input.actions["die"].IsPressed()) entity.Health -= Time.deltaTime / debugDeathTime;
#endif

        HandleCameraNoise();

        LastPosition = transform.position;
        surroundingLight.transform.position = Vector3.Lerp(surroundingLight.transform.position, transform.position, 10f * Time.deltaTime);

        Vector2 move = input.actions["move"].ReadValue<Vector2>().normalized;
        rb.AddForce(moveSpeed * 1000 * (speedBoost ? speedBoostMultiplier : 1) * Time.deltaTime * move);
        lastMove = move;
        movingAudio.volume = Mathf.Lerp(movingAudio.volume, Mathf.Clamp01(move.magnitude) * constantSoundVolume, 10f * Time.deltaTime);
        movingAudio.pitch = Mathf.Lerp(movingAudio.pitch, Mathf.Clamp01(move.magnitude), 10f * Time.deltaTime);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        float angleRad = Mathf.Atan2(
           transform.position.x - mousePos.x,
            mousePos.y - transform.position.y);
        float angleDeg = 180 / Mathf.PI * angleRad;
        Quaternion newRot = Quaternion.Euler(0, 0, angleDeg);
        rotatingAudio.volume = Mathf.Lerp(rotatingAudio.volume,
            Mathf.Clamp01(Quaternion.Angle(transform.rotation, newRot)) * constantSoundVolume, 100f * Time.deltaTime);
        transform.rotation = newRot;

        HandleDashes();

        if (Gun != null)
        {
            if (input.actions["switchfiremode"].WasPressedThisFrame() && availableBulletTypes.Count >= 2)
            {
                Gun.bulletType = Gun.bulletType == BulletType.Explosive ? BulletType.Freezing : BulletType.Explosive;
                SmallText.Appear(transform.position, $"Switched to {Gun.bulletType}", Color.white);
            }
            //#if UNITY_EDITOR
            //if (input.actions["switchfiremode"].WasPressedThisFrame()) Gun.SwitchFireMode();
            //#endif
            bool trig = input.actions["shoot"].IsPressed();
            if (Gun.triggerStatus != trig) Gun.UpdateTrigger(trig);
        }

        speedBoost = speedBoostTimeRemaining > 0 && !speedBoost;

        HandleFullVision();
    }

    #endregion

    void HandleFullVision()
    {
        if (fullVisionTimeRemaining > 0)
        {
            if (fullVision) return;
            fullVision = true;
            DOTween.To(() => surroundingLight.pointLightOuterRadius, x => surroundingLight.pointLightOuterRadius = x,
                100f, 2f);
            return;
        }
        if (!fullVision)
        {
            surroundingLight.pointLightOuterRadius = Mathf.Clamp(
                surroundingLight.pointLightOuterRadius - (Time.deltaTime * lightReductionRate), minLightOuter,
                maxLightOuter);
            return;
        }

        if (fullVisionTweenInProgress != null && fullVisionTweenInProgress.active) return;
        fullVisionTweenInProgress = DOTween.To(() => surroundingLight.pointLightOuterRadius,
                x => surroundingLight.pointLightOuterRadius = x, minLightOuter, 1f)
            .OnComplete(() => fullVision = false);
    }

    void HandleDashes()
    {
        timeSinceLastDash += Time.deltaTime;
        dashCooldownFill.fillAmount = 1f - (timeSinceLastDash / dashCooldown);

        if (availableDashes < MaxDashes)
        {
            if (timeSinceLastDash >= dashCooldown)
                currentDashReload += Time.deltaTime;

            if (currentDashReload >= dashReloadTime)
            {
                currentDashReload = 0f;
                availableDashes++;
                audioSource.PlayOneShot(AudioClipsManager.Instance.Charge);
            }
        }
        DashChargeUIController.Instance.UpdateUnits(availableDashes, currentDashReload / dashReloadTime);

        if (!input.actions["dash"].WasPressedThisFrame())
            return;

        if (availableDashes <= 0 || lastMove.magnitude <= 0)
        {
            audioSource.PlayOneShot(AudioClipsManager.Instance.CantDash);
            //Instantiate(fart, transform.position, transform.rotation);
            return;
        }

        audioSource.PlayOneShot(AudioClipsManager.Instance.Dash);
        rb.AddForce(dashForce * lastMove, ForceMode2D.Impulse);
        availableDashes--;
        timeSinceLastDash = 0f;
    }

    private void HandleCameraNoise()
    {
        float healthDiff = entity.Health / entity.MaxHealth;
        cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(cameraMinSize, cameraMaxSize, healthDiff);
        float d = (1f - healthDiff) * screenShakeIntensity;
        camNoise.m_AmplitudeGain = d;
        camNoise.m_FrequencyGain = d;
    }

    IEnumerator EverySecond()
    {
        for (; ; )
        {
            if (speedBoostTimeRemaining > 0) speedBoostTimeRemaining -= 1;
            if (fullVisionTimeRemaining > 0) fullVisionTimeRemaining -= 1;
            yield return new WaitForSeconds(1);
        }
    }
}
