using UnityEngine;
using UnityEngine.Audio;

public class OneShotSoundsCreator : PersistentSingleton<OneShotSoundsCreator>
{
    [SerializeField] AudioMixer mixer;

    protected override void Awake()
    {
        base.Awake();
    }
    
    public static void PlayOneShotAtPosition(Vector3 pos, AudioClip clip, float pitch = 1f, float volume = 1f)
    {
        GameObject go = new GameObject("One shot audio");
        go.transform.position = pos;
        PlaySound(go, clip, true, pitch, volume);
    }
    
    internal static void PlayOneShot(AudioClip clip, float pitch = 1f, float volume = 1f)
    {
        GameObject go = new GameObject("One shot audio");
        PlaySound(go, clip, false, pitch, volume);
    }
    
    internal void BulletImpact(Vector3 pos) => PlayOneShotAtPosition(pos, AudioClipsManager.Instance.BulletImpact, 1f, 5f);
    
    private static void PlaySound(GameObject go, AudioClip clip, bool spatialBlend, float pitch = 1f, float volume = 1f)
    {
        AudioSource source = go.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = Instance.mixer.FindMatchingGroups("Master/Sounds")[0]
                                       ?? Instance.mixer.FindMatchingGroups("Master")[0] ?? null;
        source.clip = clip;
        // TODO fix this volume
        source.volume = spatialBlend ? volume : volume*0.25f;
        source.spatialBlend = spatialBlend ? 1f : 0f;
        source.pitch = pitch;
        source.playOnAwake = false;
        source.Play();
        // this was from the unity audiosource source code
        Destroy(go, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }
}
