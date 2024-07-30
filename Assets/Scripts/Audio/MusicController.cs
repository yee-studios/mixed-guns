using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class MusicController : Singleton<MusicController>
{
    [SerializeField] AudioSource fullSource;
    [SerializeField] AudioSource instSource;
    [SerializeField] AudioSource deathSource;
    [SerializeField] AudioSource shopSource;

    [SerializeField, Range(0f, 1f)]
    float mixAmount = 0f;
    [SerializeField] float fadeRatio = 10f;
    [SerializeField, Range(0f, 1f)]
    float volume = 1f;
    Tween shopTween;

    protected override void Awake()
    {
        base.Awake();
        deathSource.volume = 0f;
        shopSource.volume = 0f;
        DOTween.To(() => volume, x => volume = x, 1f, 3f).ChangeStartValue(0f);
    }

    public void DeathMusic()
    {
        //fullSource.DOPitch(0f, 1f).SetUpdate(true);
        //instSource.DOPitch(0f, 1f).SetUpdate(true);
        DOTween.To(() => volume, x => volume = x, 0f, 1f).SetUpdate(true);
        deathSource.Play();
        Fade(deathSource);
    }

    public void ShopMusic(bool toggle)
    {
        //fullSource.DOPitch(toggle ? 0f : 1f, 1f).SetUpdate(true);
        //instSource.DOPitch(toggle ? 0f : 1f, 1f).SetUpdate(true);
        DOTween.To(() => volume, x => volume = x, toggle ? 0f : 1f, 1f).SetUpdate(true);
        if (toggle) shopSource.Play();
        else
        {
            if(!fullSource.isPlaying) fullSource.Play();
            if (!instSource.isPlaying) instSource.Play();
        }
        if (shopTween != null && shopTween.active) shopTween.Kill(false);
        shopTween = Fade(shopSource, 1f, toggle ? 1f : 0f, toggle ? 0f : 1f).OnComplete(() => { if (!toggle) shopSource.Stop(); });
    }

    private void Update()
    {
        Entity e = PlayerController.Instance?.Entity;
        if(e) {
            float h = e.Health / e.MaxHealth;
            mixAmount = Mathf.Clamp01((1f-h)*1.25f); //Mathf.Lerp(mixAmount, 1f-h, fadeRatio*Time.deltaTime);
        }
        float vol = Mathf.Lerp(fullSource.volume, mixAmount, fadeRatio * Time.deltaTime);
        fullSource.volume = vol*volume;
        instSource.volume = (1f-vol) * volume;
    }

    TweenerCore<float, float, FloatOptions> Fade(AudioSource source, float t = 1f, float vol = 1f, float startVol = 0f, Ease ease = Ease.InOutSine)
        => source.DOFade(vol, t).SetEase(ease)/*.ChangeStartValue(startVol)*/.SetUpdate(true);
}
