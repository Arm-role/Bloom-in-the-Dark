using UnityEngine;
using UnityEngine.Audio;

public interface IAudioLibrary
{
  AudioMixerGroup MusicGroup { get; }
  AudioMixerGroup SFXGroup { get; }

  void BuildLookups();
  ISoundData GetSound(SoundKey key);
  IMusicData GetMusic(MusicKey key);
  AudioMixerSnapshot GetSnapshot(AudioSnapshotKey key);
  AudioMixerGroup GetMixerGroup(AudioMixerGroupType type);
  void SetVolume(string param, float normalized);
  void SetMasterVolume(float v);
  void SetMusicVolume(float v);
  void SetSFXVolume(float v);
  void SetAmbientVolume(float v);
}
public enum AudioMixerGroupType
{
  Master,
  Music,
  SFX,
  Ambient,
  UI
}

public interface ISoundData
{
  bool Loop { get; }
  float MaxDistance { get; }
  bool Is3D { get; }
  AudioMixerGroupType MixerGroup { get; }

  AudioClip GetClip();
  float GetPitch();
  float GetVolume();
}

public interface IMusicData
{
  AudioClip Clip { get; }
  bool Loop { get; }
  float FadeInDuration { get; }
  float BaseVolume { get; }
}