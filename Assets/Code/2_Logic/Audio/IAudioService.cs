using UnityEngine;
/// <summary>
/// Core audio service — play SFX / Music / ambient sounds.
/// ใช้ SoundKey (ScriptableObject) เป็น ID เหมือน ItemKey/StatKey ในระบบเดิม
/// </summary>
public interface IAudioService
{
  // ------ SFX ------
  void PlaySFX(SoundKey key, Vector3 worldPosition = default);
  void PlaySFX(SoundKey key, Transform followTarget);
  void StopSFX(SoundKey key);

  // ------ Music ------
  void PlayMusic(MusicKey key, bool fadeIn = true);
  void StopMusic(bool fadeOut = true);
  void PauseMusic();
  void ResumeMusic();

  // ------ Volume (AudioMixer) ------
  void SetMasterVolume(float normalizedValue);   // 0–1
  void SetMusicVolume(float normalizedValue);
  void SetSFXVolume(float normalizedValue);
  void SetAmbientVolume(float normalizedValue);

  // ------ Snapshot / Ducking ------
  void TransitionToSnapshot(AudioSnapshotKey snapshotKey, float transitionTime = 0.5f);
}
