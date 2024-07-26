using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicController : MonoBehaviour
{
    [SerializeField] AudioSource fullSource;
    [SerializeField] AudioSource instSource;

    [SerializeField, Range(0f, 1f)]
    float mixAmount = 0f;
    [SerializeField] float fadeRatio = 10f;

    private void Awake()
    {
        FadeIn(fullSource, 0f, 0f);
        FadeIn(instSource);
    }

    private void Update()
    {
        float vol = Mathf.Lerp(fullSource.volume, mixAmount, fadeRatio * Time.deltaTime);
        fullSource.volume = vol;
        instSource.volume = 1f-vol;
    }

    void FadeIn(AudioSource source, float t = 1f, float vol = 1f, float startVol = 0f, Ease ease = Ease.InOutSine)
    {
        source.volume = startVol;
        source.DOFade(vol, t).SetEase(ease);
    }
}
