using UnityEngine;

public class AudioClipsManager : PersistentSingleton<AudioClipsManager>
{
    [field: Header("Audio Clips")]
    [field: Space(5)]
    
    [field: Header("Player")]
    [field: SerializeField] public AudioClip Dash { get; private set; }
    [field: SerializeField] public AudioClip Charge { get; private set; }
    [field: SerializeField] public AudioClip CantDash { get; private set; }
    [field: SerializeField] public AudioClip PlayerDeath { get; private set; }

    
    [field: Header("Death screen")]
    [field: SerializeField] public AudioClip Click { get; private set; }
    [field: SerializeField] public AudioClip ScreenDeath { get; private set; }
    [field: SerializeField] public AudioClip Swoosh { get; private set; }
    [field: SerializeField] public AudioClip BreathIn { get; private set; }
    [field: SerializeField] public AudioClip BreathOut { get; private set; }

    
    [field: Header("Gun play")]
    [field: SerializeField] public AudioClip BulletImpact { get; private set; }
    [field: SerializeField] public AudioClip[] Shoot  { get; private set; }
    [field: SerializeField] public AudioClip FireModeSwitch { get; private set; }
    
    [field: Header("Enemies")]
    [field: SerializeField] public AudioClip Hit { get; private set; }
    [field: SerializeField] public AudioClip EnemyDeath { get; private set; }
}
