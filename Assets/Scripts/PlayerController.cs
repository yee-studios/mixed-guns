using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Singleton<PlayerController>
{
    #region cached components
    Rigidbody2D rb;
    PlayerInput input;
    AudioSource audioSource;
    Entity entity;
    public Entity Entity => entity;
    Vector2 lastMove = new();
    public Vector2 LastPosition { get; private set; }
    #endregion

    [Header("Parameters")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float dashForce = 1000f;
    [SerializeField] int availableDashes = 0;
    [SerializeField] int maxDashes = 3;
    [Range(0f,1f)]
    [SerializeField] float constantSoundVolume = 0.25f;
    [SerializeField] float lightReductionRate = 0.25f;
    [SerializeField] float minLightOuter = 3f;
    [Range(0f, 5f)]
    [SerializeField] float screenShakeIntensity = 1.5f;
    public bool startAnimation { private set; get; }

    [Header("Audio Sources")]
    [SerializeField] AudioSource movingAudio;
    [SerializeField] AudioSource rotatingAudio;

    [SerializeField] ParticleSystem fart;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    CinemachineBasicMultiChannelPerlin cam_noise;

    [SerializeField] GunController gun;
    public Light2D flashLight;
    public Light2D surroundingLight;

    public int MaxDashes => maxDashes;
    [SerializeField] float dashReloadTime = 1f;
    [SerializeField] float currentDashReload = 0f;

    [SerializeField] float debugDeathTime = 0f;

    #region unity methods
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        entity = GetComponent<Entity>();
        entity.OnDied.AddListener(OnDied);
        cam_noise = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    float d1 = 3f;
    float d2 = .25f;

    protected void Start()
    {
        input = FindObjectOfType<PlayerInput>();
        startAnimation = true;
        entity.invencibility = true;
        DOTween.To(() => surroundingLight.falloffIntensity, x => surroundingLight.falloffIntensity = x, 0.5f, d1)
            .ChangeStartValue(1f).SetDelay(d2).OnComplete(() => { entity.invencibility = false; startAnimation = false; });    

        DOTween.To(() => minLightOuter, x => minLightOuter = x, 3f, d1).ChangeStartValue(0f).SetDelay(d2);
        DOTween.To(() => surroundingLight.pointLightOuterRadius, x => surroundingLight.pointLightOuterRadius = x,
            surroundingLight.pointLightOuterRadius, d1).ChangeStartValue(0f).SetDelay(d2).SetUpdate(UpdateType.Late);

        DOTween.To(() => flashLight.falloffIntensity, x => flashLight.falloffIntensity = x, 0.5f, d1).ChangeStartValue(1f).SetDelay(d2);
        DOTween.To(() => flashLight.pointLightInnerRadius, x => flashLight.pointLightInnerRadius = x,
            flashLight.pointLightInnerRadius, d1).ChangeStartValue(0f).SetDelay(d2);
        DOTween.To(() => flashLight.pointLightOuterRadius, x => flashLight.pointLightOuterRadius = x,
            flashLight.pointLightOuterRadius, d1).ChangeStartValue(0f).SetDelay(d2);
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
#if UNITY_EDITOR
        if (input.actions["die"].IsPressed()) entity.Health -= Time.deltaTime/debugDeathTime;
#endif

        if (ShopController.Instance.shopOpen) return;

        HandleCameraNoise();

        LastPosition = transform.position;
        surroundingLight.transform.position = Vector3.Lerp(surroundingLight.transform.position, transform.position, 10f * Time.deltaTime);

        Vector2 move = input.actions["move"].ReadValue<Vector2>().normalized;
        rb.AddForce(moveSpeed * Time.deltaTime * move);
        lastMove = move;
        movingAudio.volume = Mathf.Lerp(movingAudio.volume, Mathf.Clamp01(move.magnitude) * constantSoundVolume, 10f*Time.deltaTime);
        movingAudio.pitch = Mathf.Lerp(movingAudio.pitch, Mathf.Clamp01(move.magnitude), 10f * Time.deltaTime);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        float angleRad = Mathf.Atan2(
           transform.position.x - mousePos.x,
            mousePos.y - transform.position.y);
        float angleDeg = 180 / Mathf.PI * angleRad;
        Quaternion newRot = Quaternion.Euler(0, 0, angleDeg);
        rotatingAudio.volume = Mathf.Lerp(rotatingAudio.volume,
            Mathf.Clamp01(Quaternion.Angle(transform.rotation, newRot)) * constantSoundVolume, 100f*Time.deltaTime);
        transform.rotation = newRot;

        HandleDashes();

        if(gun != null)
        {
            if (input.actions["switchfiremode"].WasPressedThisFrame()) gun.SwitchFireMode();
            bool trig = input.actions["shoot"].IsPressed();
            if (gun.triggerStatus != trig) gun.UpdateTrigger(trig);
        }

        surroundingLight.pointLightOuterRadius = Mathf.Clamp(
            surroundingLight.pointLightOuterRadius-(Time.deltaTime*lightReductionRate), minLightOuter, Mathf.Infinity);
    }

    #endregion

    void HandleDashes()
    {
        if (availableDashes < maxDashes)
        {
            currentDashReload += Time.deltaTime;
            if (currentDashReload >= dashReloadTime)
            {
                currentDashReload = 0f;
                availableDashes++;
                audioSource.PlayOneShot(AudioClipsManager.Instance.Charge);
            }
        }
        DashChargeUIController.Instance.UpdateUnits(availableDashes, currentDashReload/dashReloadTime);

        if (!input.actions["dash"].WasPressedThisFrame())
            return;

        if (availableDashes <= 0 || lastMove.magnitude <= 0)
        {
            audioSource.PlayOneShot(AudioClipsManager.Instance.CantDash);
            Instantiate(fart, transform.position, transform.rotation);
            return;
        }

        audioSource.PlayOneShot(AudioClipsManager.Instance.Dash);
        rb.AddForce(dashForce * lastMove, ForceMode2D.Impulse);
        availableDashes--;
    }

    private void HandleCameraNoise()
    {
        float healthDiff = entity.Health / entity.MaxHealth;
        cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(3f, 4f, healthDiff);
        float d = (1f-healthDiff) * screenShakeIntensity;
        cam_noise.m_AmplitudeGain = d;
        cam_noise.m_FrequencyGain = d;
    }
}
