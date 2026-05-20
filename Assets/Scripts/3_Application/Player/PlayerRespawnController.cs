using System;
using System.Collections;
using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
  [Header("Refs")]
  [SerializeField] private PlayerController _player;
  [SerializeField] private AltarController _altar;
  [SerializeField] private CameraController _camera;
  [SerializeField] private MonoBehaviour _viewBehaviour;

  [Header("Settings")]
  [SerializeField] private float _respawnDuration = 5f;
  [Range(0f, 1f)]
  [SerializeField] private float _hpRecoveryPercent = 0.3f;
  [SerializeField] private Vector2 _spawnOffset;

  public event Action OnRespawnStarted;
  public event Action<float> OnRespawnCountdown;
  public event Action OnRespawnCompleted;

  public bool IsRespawning { get; private set; }

  private IRespawnView _view;

  private void Start()
  {
    if (_player == null)
    {
      Debug.LogWarning("[PlayerRespawnController] _player is not assigned");
      return;
    }

    _view = _viewBehaviour as IRespawnView;
    if (_viewBehaviour != null && _view == null)
      Debug.LogWarning("[PlayerRespawnController] _viewBehaviour does not implement IRespawnView");

    _view?.Hide();
    _player.OnPlayerDied += HandlePlayerDied;
  }

  private void OnDestroy()
  {
    if (_player != null)
      _player.OnPlayerDied -= HandlePlayerDied;
  }

  private void HandlePlayerDied()
  {
    if (IsRespawning) return;
    StartCoroutine(RespawnFlow());
  }

  // เรียกตอน GameOver (เช่น base พัง) เพื่อหยุด respawn flow + คืน state
  public void CancelRespawn()
  {
    if (!IsRespawning) return;

    StopAllCoroutines();

    if (_camera != null)
      _camera.SetState(CameraState.Follow);

    _view?.Hide();
    _player?.Interactor?.ReleaseLock();
    GameSession.IsPlayerRespawning = false;
    IsRespawning = false;
  }

  private IEnumerator RespawnFlow()
  {
    IsRespawning = true;
    GameSession.IsPlayerRespawning = true;
    _player.Interactor?.SetExclusiveLock("respawn", float.PositiveInfinity);

    if (_camera != null)
      _camera.SetState(CameraState.FreePan);

    _view?.Show();
    _view?.SetCountdown(_respawnDuration);

    OnRespawnStarted?.Invoke();

    float remaining = _respawnDuration;
    while (remaining > 0f)
    {
      _view?.SetCountdown(remaining);
      OnRespawnCountdown?.Invoke(remaining);
      yield return null;
      remaining -= Time.deltaTime;
    }
    _view?.SetCountdown(0f);
    OnRespawnCountdown?.Invoke(0f);

    Respawn();

    if (_camera != null)
      _camera.SetState(CameraState.Follow);

    _view?.Hide();

    _player.Interactor?.ReleaseLock();
    GameSession.IsPlayerRespawning = false;
    IsRespawning = false;

    OnRespawnCompleted?.Invoke();
  }

  private void Respawn()
  {
    Vector3 pos = _altar != null
      ? _altar.transform.position + (Vector3)_spawnOffset
      : _player.transform.position;

    _player.Respawn(pos, _hpRecoveryPercent);
  }
}
