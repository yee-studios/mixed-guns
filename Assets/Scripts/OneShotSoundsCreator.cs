using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class OneShotSoundsCreator : PersistentSingleton<OneShotSoundsCreator>
{
    [SerializeField] AudioClip bulletImpact;
    [SerializeField] AudioMixer mixer;

    protected override void Awake()
    {
        base.Awake();
    }

    // TODO this is experimental
    public static void CreateOneShotAtPosition(Vector3 pos, AudioClip clip, float pitch = 1f, float volume = 1f)
    {
        GameObject go = new GameObject("One shot audio");
        go.transform.position = pos;
        AudioSource source = go.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = Instance.mixer.FindMatchingGroups("Master/Sounds")[0]
            ?? Instance.mixer.FindMatchingGroups("Master")[0] ?? null;
        source.clip = clip;
        source.spatialBlend = 1f;
        source.volume = volume;
        source.pitch = pitch;
        source.playOnAwake = false;
        source.Play();
        // this was from the unity audiosource source code
        Destroy(go, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }

    internal void BulletImpact(Vector3 pos) => CreateOneShotAtPosition(pos, bulletImpact, 1f, 5f);

    // TODO reuse code
    internal static void PlaySound(AudioClip clip, float pitch = 1f, float volume = 1f)
    {
        GameObject go = new GameObject("One shot audio");
        AudioSource source = go.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = Instance.mixer.FindMatchingGroups("Master/Sounds")[0]
            ?? Instance.mixer.FindMatchingGroups("Master")[0] ?? null;
        source.clip = clip;
        // TODO fix this volume
        source.volume = volume*0.25f;
        source.pitch = pitch;
        source.playOnAwake = false;
        source.Play();
        // this was from the unity audiosource source code
        Destroy(go, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }
}
