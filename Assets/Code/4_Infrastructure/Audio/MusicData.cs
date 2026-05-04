using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Music Data")]
public class MusicData : ScriptableObject, IMusicData
{
  [Header("Key")]
  [SerializeField] private MusicKey _key;

  [Header("Clip")]
  [SerializeField] private AudioClip _clip;

  [Header("Volume")]
  [Range(0f, 1f)]
  [SerializeField] private float _baseVolume = 0.8f;

  [Header("Fade")]
  [SerializeField] private float _fadeInDuration = 1f;
  [SerializeField] private float _fadeOutDuration = 1f;

  [SerializeField] private bool _loop = true;

  public MusicKey Key => _key;

  public AudioClip Clip => _clip;
  public bool Loop => _loop;
  public float FadeInDuration => _fadeInDuration;
  public float FadeOutDuration => _fadeOutDuration;
  public float BaseVolume => _baseVolume;
}