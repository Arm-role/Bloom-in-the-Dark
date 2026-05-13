using UnityEngine;
/// <summary>
/// ตัวกลางสำหรับ UI (Slider) → AudioService
/// ใช้ PlayerPrefs เพื่อ persist ค่าระหว่าง session
/// </summary>
public class AudioVolumeController
{
  private readonly IAudioService _audioService;

  private const string KEY_MASTER = "Vol_Master";
  private const string KEY_MUSIC = "Vol_Music";
  private const string KEY_SFX = "Vol_SFX";
  private const string KEY_AMBIENT = "Vol_Ambient";

  public float MasterVolume { get; private set; }
  public float MusicVolume { get; private set; }
  public float SFXVolume { get; private set; }
  public float AmbientVolume { get; private set; }

  public AudioVolumeController(IAudioService audioService)
  {
    _audioService = audioService;
    LoadAndApplyAll();
  }

  // ---- Public API (ผูกกับ Slider OnValueChanged) ----

  public void SetMaster(float v)
  {
    MasterVolume = v;
    _audioService.SetMasterVolume(v);
    PlayerPrefs.SetFloat(KEY_MASTER, v);
  }

  public void SetMusic(float v)
  {
    MusicVolume = v;
    _audioService.SetMusicVolume(v);
    PlayerPrefs.SetFloat(KEY_MUSIC, v);
  }

  public void SetSFX(float v)
  {
    SFXVolume = v;
    _audioService.SetSFXVolume(v);
    PlayerPrefs.SetFloat(KEY_SFX, v);
  }

  public void SetAmbient(float v)
  {
    AmbientVolume = v;
    _audioService.SetAmbientVolume(v);
    PlayerPrefs.SetFloat(KEY_AMBIENT, v);
  }

  // ---- Load saved values ----

  private void LoadAndApplyAll()
  {
    SetMaster(PlayerPrefs.GetFloat(KEY_MASTER, 1f));
    SetMusic(PlayerPrefs.GetFloat(KEY_MUSIC, 1f));
    SetSFX(PlayerPrefs.GetFloat(KEY_SFX, 1f));
    SetAmbient(PlayerPrefs.GetFloat(KEY_AMBIENT, 1f));
  }
}