using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
  [SerializeField] private MusicKey _music;
  [SerializeField] private bool _fadeIn = true;

  private void Start()
  {
    if (_music == null) return;
    if (AudioBootstrap.Service == null)
    {
      Debug.LogWarning("[SceneMusicTrigger] AudioBootstrap.Service is null — make sure AudioBootstrap exists in scene");
      return;
    }
    AudioBootstrap.Service.PlayMusic(_music, _fadeIn);
  }
}
