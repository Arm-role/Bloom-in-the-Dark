using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject, ISoundData
{
  [Header("Key")]
  [SerializeField] private SoundKey _key;

  [Header("Clips (random pick)")]
  [SerializeField] private AudioClip[] _clips;

  [Header("Mixer Group")]
  [SerializeField] private AudioMixerGroupType _mixerGroup = AudioMixerGroupType.SFX;

  [Header("Volume")]
  [Range(0f, 1f)]
  [SerializeField] private float _baseVolume = 1f;
  [Range(0f, 0.3f)]
  [SerializeField] private float _volumeVariance = 0f;

  [Header("Pitch")]
  [Range(0.5f, 2f)]
  [SerializeField] private float _basePitch = 1f;
  [Range(0f, 0.3f)]
  [SerializeField] private float _pitchVariance = 0.1f;

  [Header("Spatial")]
  [SerializeField] private bool _is3D = false;
  [Range(0f, 500f)]
  [SerializeField] private float _maxDistance = 20f;

  [Header("Flags")]
  [SerializeField] private bool _loop = false;
  [SerializeField] private bool _playOnAwake = false;

  public SoundKey Key => _key;
  public bool PlayOnAwake => _playOnAwake;

  public AudioMixerGroupType MixerGroup => _mixerGroup;
  public bool Loop => _loop;
  public bool Is3D => _is3D;
  public float MaxDistance => _maxDistance;

  // ---- Computed ----

  public AudioClip GetClip()
  {
    if (_clips == null || _clips.Length == 0) return null;
    return _clips[Random.Range(0, _clips.Length)];
  }

  public float GetVolume()
      => Mathf.Clamp01(_baseVolume + Random.Range(-_volumeVariance, _volumeVariance));

  public float GetPitch()
      => Mathf.Clamp(_basePitch + Random.Range(-_pitchVariance, _pitchVariance), 0.5f, 2f);
}