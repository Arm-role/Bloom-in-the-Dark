using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// Library กลางสำหรับเก็บ SoundData + MusicData ทั้งหมด
/// ใช้ Hash ของ SoundKey/MusicKey เป็น index — เหมือน LibraryBase<T> เดิม
/// </summary>
[CreateAssetMenu(menuName = "Library/AudioLibrary")]
public class AudioLibrary : ScriptableObject, IAudioLibrary
{
  [Header("Sounds (SFX / Ambient)")]
  [SerializeField] private List<SoundData> _sounds = new();

  [Header("Music Tracks")]
  [SerializeField] private List<MusicData> _musicTracks = new();

  [Header("Audio Mixer")]
  [SerializeField] private AudioMixer _mixer;

  [Header("Mixer Group References")]
  [SerializeField] private AudioMixerGroup _masterGroup;
  [SerializeField] private AudioMixerGroup _musicGroup;
  [SerializeField] private AudioMixerGroup _SFXGroup;
  [SerializeField] private AudioMixerGroup _ambientGroup;
  [SerializeField] private AudioMixerGroup _UIGroup;

  [Header("Snapshots")]
  public List<AudioMixerSnapshot> Snapshots = new();

  // ---- Lookup ----

  private Dictionary<int, ISoundData> _soundLookup;
  private Dictionary<int, IMusicData> _musicLookup;
  private Dictionary<int, AudioMixerSnapshot> _snapshotLookup;

  public AudioMixerGroup MusicGroup => _masterGroup;
  public AudioMixerGroup SFXGroup => _SFXGroup;

  private void OnEnable() => BuildLookups();

  public void BuildLookups()
  {
    _soundLookup = new();
    foreach (var s in _sounds)
    {
      if (s?.Key == null) continue;
      int hash = s.Key.RuntimeTag.Hash;
      if (!_soundLookup.ContainsKey(hash))
        _soundLookup[hash] = (ISoundData)s;
    }

    _musicLookup = new();
    foreach (var m in _musicTracks)
    {
      if (m?.Key == null) continue;
      int hash = m.Key.RuntimeTag.Hash;
      if (!_musicLookup.ContainsKey(hash))
        _musicLookup[hash] = (IMusicData)m;
    }

    _snapshotLookup = new();
    foreach (var snap in Snapshots)
    {
      if (snap == null) continue;
      int hash = snap.name.GetHashCode();
      if (!_snapshotLookup.ContainsKey(hash))
        _snapshotLookup[hash] = snap;
    }
  }

  public ISoundData GetSound(SoundKey key)
  {
    if (_soundLookup == null) BuildLookups();
    _soundLookup.TryGetValue(key.RuntimeTag.Hash, out var data);
    return data;
  }

  public IMusicData GetMusic(MusicKey key)
  {
    if (_musicLookup == null) BuildLookups();
    _musicLookup.TryGetValue(key.RuntimeTag.Hash, out var data);
    return data;
  }

  public AudioMixerSnapshot GetSnapshot(AudioSnapshotKey key)
  {
    if (_snapshotLookup == null) BuildLookups();
    int hash = key.name.GetHashCode();
    _snapshotLookup.TryGetValue(hash, out var snap);
    return snap;
  }

  public AudioMixerGroup GetMixerGroup(AudioMixerGroupType type) => type switch
  {
    AudioMixerGroupType.Music => _musicGroup,
    AudioMixerGroupType.SFX => _SFXGroup,
    AudioMixerGroupType.Ambient => _ambientGroup,
    AudioMixerGroupType.UI => _UIGroup,
    _ => _masterGroup
  };

  // ---- Mixer Volume Helpers ----
  // AudioMixer รับ dB (-80 ถึง 0) แต่ expose เป็น 0-1 ให้ service

  private const string MASTER_PARAM = "MasterVolume";
  private const string MUSIC_PARAM = "MusicVolume";
  private const string SFX_PARAM = "SFXVolume";
  private const string AMBIENT_PARAM = "AmbientVolume";

  public void SetVolume(string param, float normalized)
  {
    float dB = normalized <= 0.001f
        ? -80f
        : Mathf.Log10(normalized) * 20f;

    _mixer?.SetFloat(param, dB);
  }

  public void SetMasterVolume(float v) => SetVolume(MASTER_PARAM, v);
  public void SetMusicVolume(float v) => SetVolume(MUSIC_PARAM, v);
  public void SetSFXVolume(float v) => SetVolume(SFX_PARAM, v);
  public void SetAmbientVolume(float v) => SetVolume(AMBIENT_PARAM, v);
}