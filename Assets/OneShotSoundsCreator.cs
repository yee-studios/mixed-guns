using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Audio;
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
    public static void CreateOneShotAtPosition(Vector3 pos, AudioClip audioClip, float pitch, float volume)
    {
        AudioSource.PlayClipAtPoint(audioClip, pos);
        return;
#pragma warning disable CS0162 // Unreachable code detected
        GameObject go = new GameObject();
#pragma warning restore CS0162 // Unreachable code detected
        AudioSource source = go.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = Instance.mixer.FindMatchingGroups("Master/Sounds")[0]
            ?? Instance.mixer.FindMatchingGroups("Master")[0] ?? null;
        source.clip = audioClip;
        source.volume = volume;
        source.pitch = pitch;
        source.playOnAwake = false;
        source.Play();
    }

    internal void BulletImpact(Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(bulletImpact, pos);
    }
}
