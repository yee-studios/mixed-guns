using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] int maxDashes = 3;
    public int MaxDashes => maxDashes;
    [SerializeField] float dashReloadTime = 1f;
    [SerializeField] float currentDashReload = 0f;
    
    [Header("Lightning")]
    [Range(0f,1f)]
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
    public int doubleSpeedTimeRemaining;
    public int fullVisionTimeRemaining;

    
    public bool startAnimation { private set; get; }

    [Header("Audio")]
    [SerializeField] float constantSoundVolume = 0.25f;
    [SerializeField] AudioSource movingAudio;
    [SerializeField] AudioSource rotatingAudio;

    [SerializeField] ParticleSystem fart;

    [Header("Other")]
    [SerializeField] GunController gun;

    [SerializeField] float debugDeathTime = 0f;
    #endregion
    
    bool doubleSpeed;
    bool fullVision;
    bool fullVisionTweenInProgress;

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
        startAnimation = true;
        entity.invencibility = true;
        DOTween.To(() => surroundingLight.falloffIntensity, x => surroundingLight.falloffIntensity = x, 0.5f, delay1)
            .ChangeStartValue(1f).SetDelay(delay2).OnComplete(() => { entity.invencibility = false; startAnimation = false; });    

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
#if UNITY_EDITOR
        if (input.actions["die"].IsPressed()) entity.Health -= Time.deltaTime/debugDeathTime;
#endif
        if (ShopController.Instance.shopOpen) return;

        HandleCameraNoise();

        LastPosition = transform.position;
        surroundingLight.transform.position = Vector3.Lerp(surroundingLight.transform.position, transform.position, 10f * Time.deltaTime);

        Vector2 move = input.actions["move"].ReadValue<Vector2>().normalized;
        rb.AddForce(moveSpeed * 1000 * (doubleSpeed ? 2 : 1) * Time.deltaTime * move);
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
        
        if (doubleSpeedTimeRemaining > 0 && !doubleSpeed) doubleSpeed = true;
        else if (doubleSpeedTimeRemaining <= 0 && doubleSpeed) doubleSpeed = false;

        if (fullVisionTimeRemaining > 0) {
            if (fullVision) return;
            fullVision = true;
            DOTween.To(() => surroundingLight.pointLightOuterRadius, x => surroundingLight.pointLightOuterRadius = x,
                100f, 2f);
        }
        else
        {
            if (!fullVision)
            {
                surroundingLight.pointLightOuterRadius = Mathf.Clamp(
                    surroundingLight.pointLightOuterRadius - (Time.deltaTime * lightReductionRate), minLightOuter,
                    maxLightOuter);
                
                return;
            }

            if (fullVisionTweenInProgress) return;
            fullVisionTweenInProgress = true;
            DOTween.To(() => surroundingLight.pointLightOuterRadius,
                    x => surroundingLight.pointLightOuterRadius = x, minLightOuter, 1f)
                .OnComplete(() =>
                {
                    fullVision = false;
                    fullVisionTweenInProgress = false;
                });
        }
        
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
        cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(cameraMinSize, cameraMaxSize, healthDiff);
        float d = (1f-healthDiff) * screenShakeIntensity;
        camNoise.m_AmplitudeGain = d;
        camNoise.m_FrequencyGain = d;
    }

    IEnumerator EverySecond() {
        for (;;)
        {
            if (doubleSpeedTimeRemaining > 0) doubleSpeedTimeRemaining -= 1;
            if (fullVisionTimeRemaining > 0) fullVisionTimeRemaining -= 1;
            yield return new WaitForSeconds(1);
        }
    }
}
