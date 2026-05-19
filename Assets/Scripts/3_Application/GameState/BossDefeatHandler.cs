using System.Collections;
using UnityEngine;

public class BossDefeatHandler : MonoBehaviour
{
  [SerializeField] private MonoBehaviour _cycleControllerBehaviour;
  [SerializeField] private EndGameController _endGameController;
  [SerializeField] private float _delayBeforeShow = 1.5f;

  private ICycleController _cycle;

  private void Start()
  {
    _cycle = _cycleControllerBehaviour as ICycleController;
    if (_cycle == null)
    {
      Debug.LogWarning("[BossDefeatHandler] _cycleControllerBehaviour does not implement ICycleController");
      return;
    }

    _cycle.OnBossKilled += HandleBossKilled;
  }

  private void OnDestroy()
  {
    if (_cycle != null)
      _cycle.OnBossKilled -= HandleBossKilled;
  }

  private void HandleBossKilled()
  {
    // ใน endless mode — boss ตายต่อก็ไม่ทำอะไร
    if (GameSession.IsEndlessMode) return;

    GameSession.IsEndlessMode = true;
    StartCoroutine(DelayedShow());
  }

  private IEnumerator DelayedShow()
  {
    yield return new WaitForSeconds(_delayBeforeShow);

    if (_endGameController == null)
    {
      Debug.LogWarning("[BossDefeatHandler] _endGameController is not assigned");
      yield break;
    }

    _endGameController.Show();
  }
}
