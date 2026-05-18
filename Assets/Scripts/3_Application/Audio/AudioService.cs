// ============================================================
// Assets/Code/3_Application/Audio/AudioService.cs
// ============================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Core IAudioService implementation.
/// ทำงานบน GameObject หนึ่งตัวที่ DontDestroyOnLoad
/// Pool AudioSource ภายในเพื่อไม่ต้อง Instantiate ทุกครั้ง
/// </summary>
public class AudioService : MonoBehaviour, IAudioService
{
  // ---- Dependencies (inject via Initialize) ----
  private IAudioLibrary _library;

  // ---- Music ----
  private AudioSource _musicSource;
  private AudioSource _musicSourceB;          // cross-fade target
  private bool _isMusicSourceAActive = true;

  // ---- SFX Pool ----
  private readonly Queue<AudioSource> _sfxPool = new();
  private readonly Dictionary<int, AudioSource> _loopingSfx = new();  // key hash → source
  private const int POOL_SIZE = 20;

  // ---- State ----
  private Coroutine _fadeMusicCoroutine;

  // ============================================================
  // Init
  // ============================================================

  public void Initialize(IAudioLibrary library)
  {
    _library = library;

    // สร้าง AudioSource สำหรับ Music (2 ตัวสำหรับ cross-fade)
    _musicSource = CreateAudioSource("Music_A", library.MusicGroup);
    _musicSourceB = CreateAudioSource("Music_B", library.MusicGroup);

    // สร้าง SFX pool
    for (int i = 0; i < POOL_SIZE; i++)
      _sfxPool.Enqueue(CreateAudioSource($"SFX_{i}", library.SFXGroup));
  }

  // ============================================================
  // IAudioService — SFX
  // ============================================================

  public void PlaySFX(SoundKey key, Vector3 worldPosition = default)
  {
    var data = _library.GetSound(key);
    if (data == null) { LogMissing(key.name); return; }

    var source = GetPooledSource();
    if (source == null) return;

    ConfigureSource(source, data, worldPosition);
    source.Play();

    if (!data.Loop)
      StartCoroutine(ReturnToPool(source, data.GetClip().length / source.pitch));
    else
      _loopingSfx[key.RuntimeTag.Hash] = source;
  }

  public void PlaySFX(SoundKey key, Transform followTarget)
  {
    var data = _library.GetSound(key);
    if (data == null) { LogMissing(key.name); return; }

    var source = GetPooledSource();
    if (source == null) return;

    source.transform.SetParent(followTarget);
    source.transform.localPosition = Vector3.zero;
    ConfigureSource(source, data, Vector3.zero);
    source.Play();

    if (!data.Loop)
      StartCoroutine(ReturnToPoolAndDetach(source, data.GetClip().length / source.pitch));
    else
      _loopingSfx[key.RuntimeTag.Hash] = source;
  }

  public void StopSFX(SoundKey key)
  {
    int hash = key.RuntimeTag.Hash;
    if (!_loopingSfx.TryGetValue(hash, out var source)) return;

    source.Stop();
    source.transform.SetParent(transform);
    _sfxPool.Enqueue(source);
    _loopingSfx.Remove(hash);
  }

  // ============================================================
  // IAudioService — Music
  // ============================================================

  public void PlayMusic(MusicKey key, bool fadeIn = true)
  {
    var data = _library.GetMusic(key);
    if (data == null) { LogMissing(key.name); return; }

    if (_fadeMusicCoroutine != null)
      StopCoroutine(_fadeMusicCoroutine);

    _fadeMusicCoroutine = StartCoroutine(CrossFadeMusic(data, fadeIn));
  }

  public void StopMusic(bool fadeOut = true)
  {
    if (_fadeMusicCoroutine != null)
      StopCoroutine(_fadeMusicCoroutine);

    var active = GetActiveMusicSource();
    if (fadeOut)
      _fadeMusicCoroutine = StartCoroutine(FadeOut(active, 1f));
    else
      active.Stop();
  }

  public void PauseMusic() => GetActiveMusicSource().Pause();
  public void ResumeMusic() => GetActiveMusicSource().UnPause();

  // ============================================================
  // IAudioService — Volume (delegate to AudioLibrary)
  // ============================================================

  public void SetMasterVolume(float v) => _library.SetMasterVolume(v);
  public void SetMusicVolume(float v) => _library.SetMusicVolume(v);
  public void SetSFXVolume(float v) => _library.SetSFXVolume(v);
  public void SetAmbientVolume(float v) => _library.SetAmbientVolume(v);

  // ============================================================
  // IAudioService — Snapshot / Ducking
  // ============================================================

  public void TransitionToSnapshot(AudioSnapshotKey snapshotKey, float transitionTime = 0.5f)
  {
    var snap = _library.GetSnapshot(snapshotKey);
    if (snap == null) { LogMissing(snapshotKey.name); return; }
    snap.TransitionTo(transitionTime);
  }

  // ============================================================
  // Private Helpers
  // ============================================================

  private AudioSource GetActiveMusicSource()
      => _isMusicSourceAActive ? _musicSource : _musicSourceB;

  private AudioSource GetInactiveMusicSource()
      => _isMusicSourceAActive ? _musicSourceB : _musicSource;

  private AudioSource GetPooledSource()
  {
    if (_sfxPool.Count > 0)
      return _sfxPool.Dequeue();

    Debug.LogWarning("[AudioService] SFX pool empty — consider increasing POOL_SIZE");
    return null;
  }

  private void ConfigureSource(AudioSource source, ISoundData data, Vector3 worldPos)
  {
    source.clip = data.GetClip();
    source.volume = data.GetVolume();
    source.pitch = data.GetPitch();
    source.loop = data.Loop;
    source.outputAudioMixerGroup = _library.GetMixerGroup(data.MixerGroup);
    source.spatialBlend = data.Is3D ? 1f : 0f;
    source.maxDistance = data.MaxDistance;
    source.transform.position = worldPos;
  }

  private AudioSource CreateAudioSource(string goName, UnityEngine.Audio.AudioMixerGroup group)
  {
    var go = new GameObject(goName);
    go.transform.SetParent(transform);
    var src = go.AddComponent<AudioSource>();
    src.outputAudioMixerGroup = group;
    src.playOnAwake = false;
    return src;
  }

  // ============================================================
  // Coroutines
  // ============================================================

  private IEnumerator CrossFadeMusic(IMusicData data, bool fade)
  {
    var outSource = GetActiveMusicSource();
    var inSource = GetInactiveMusicSource();
    _isMusicSourceAActive = !_isMusicSourceAActive;

    inSource.clip = data.Clip;
    inSource.loop = data.Loop;
    inSource.volume = 0f;
    inSource.Play();

    float fadeDuration = fade ? data.FadeInDuration : 0f;
    float t = 0f;
    float startVol = outSource.volume;

    while (t < fadeDuration)
    {
      t += Time.unscaledDeltaTime;
      float ratio = t / fadeDuration;
      inSource.volume = Mathf.Lerp(0f, data.BaseVolume, ratio);
      outSource.volume = Mathf.Lerp(startVol, 0f, ratio);
      yield return null;
    }

    inSource.volume = data.BaseVolume;
    outSource.Stop();
    outSource.volume = 0f;
  }

  private IEnumerator FadeOut(AudioSource source, float duration)
  {
    float startVol = source.volume;
    float t = 0f;

    while (t < duration)
    {
      t += Time.unscaledDeltaTime;
      source.volume = Mathf.Lerp(startVol, 0f, t / duration);
      yield return null;
    }

    source.Stop();
    source.volume = 0f;
  }

  private IEnumerator ReturnToPool(AudioSource source, float delay)
  {
    yield return new WaitForSeconds(delay);
    _sfxPool.Enqueue(source);
  }

  private IEnumerator ReturnToPoolAndDetach(AudioSource source, float delay)
  {
    yield return new WaitForSeconds(delay);
    source.transform.SetParent(transform);
    source.transform.localPosition = Vector3.zero;
    _sfxPool.Enqueue(source);
  }

  private static void LogMissing(string name)
      => Debug.LogWarning($"[AudioService] Key not found in AudioLibrary: '{name}'");
}

