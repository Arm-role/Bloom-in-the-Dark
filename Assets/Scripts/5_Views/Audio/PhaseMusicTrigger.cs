using UnityEngine;

public class PhaseMusicTrigger : MonoBehaviour
{
  [SerializeField] private TurnSystem _turnSystem;

  [Header("Music per Phase")]
  [SerializeField] private MusicKey _farmMusic;
  [SerializeField] private MusicKey _prepareMusic;
  [SerializeField] private MusicKey _battleMusic;

  [SerializeField] private bool _fadeIn = true;

  // subscribe ใน Awake — ก่อน TurnSystem.Initialize ถูกเรียกใน Start
  // เพื่อให้จับ OnNextTurn ครั้งแรก (defaultTurnState) ทันที
  private void Awake()
  {
    if (_turnSystem == null)
    {
      Debug.LogWarning("[PhaseMusicTrigger] _turnSystem is not assigned");
      return;
    }
    _turnSystem.OnNextTurn += HandlePhaseChange;
  }

  private void OnDestroy()
  {
    if (_turnSystem == null) return;
    _turnSystem.OnNextTurn -= HandlePhaseChange;
  }

  private void HandlePhaseChange(ETurnState state)
  {
    MusicKey key = state switch
    {
      ETurnState.Farm        => _farmMusic,
      ETurnState.Preparation => _prepareMusic,
      ETurnState.Battle      => _battleMusic,
      _                      => null
    };

    if (key == null) return;
    if (AudioBootstrap.Service == null)
    {
      Debug.LogWarning("[PhaseMusicTrigger] AudioBootstrap.Service is null");
      return;
    }

    AudioBootstrap.Service.PlayMusic(key, _fadeIn);
  }
}
