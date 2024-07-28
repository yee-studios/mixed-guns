using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicController : Singleton<MusicController>
{
    [SerializeField] AudioSource fullSource;
    [SerializeField] AudioSource instSource;
    [SerializeField] AudioSource deathSource;

    [SerializeField, Range(0f, 1f)]
    float mixAmount = 0f;
    [SerializeField] float fadeRatio = 10f;
    float volume = 1f;

    protected override void Awake()
    {
        base.Awake();
        deathSource.volume = 0f;
        DOTween.To(() => volume, x => volume = x, 1f, 3f).ChangeStartValue(0f);
    }

    public void DeathMusic()
    {
        fullSource.DOPitch(0f, 1f);
        instSource.DOPitch(0f, 1f);
        deathSource.Play();
        FadeIn(deathSource);
    }

    private void Update()
    {
        Entity e = PlayerController.Instance?.Entity;
        if(e) {
            float h = e.Health / e.MaxHealth;
            mixAmount = 1f - h; //Mathf.Lerp(mixAmount, 1f-h, fadeRatio*Time.deltaTime);
        }
        float vol = Mathf.Lerp(fullSource.volume, mixAmount, fadeRatio * Time.deltaTime);
        fullSource.volume = vol*volume;
        instSource.volume = (1f-vol) * volume;
    }

    void FadeIn(AudioSource source, float t = 1f, float vol = 1f, float startVol = 0f, Ease ease = Ease.InOutSine)
    {
        source.volume = startVol;
        source.DOFade(vol, t).SetEase(ease);
    }
}
