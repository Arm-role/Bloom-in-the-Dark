using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnView : MonoBehaviour, ITurnView
{
  [Header("UI")]
  [SerializeField] private TMP_Text dayText;
  [SerializeField] private TMP_Text turnStateText;
  [SerializeField] private Button nextTurnButton;

  [Header("Transition - Fade")]
  [SerializeField] private CanvasGroup _fadeOverlay;
  [SerializeField] private float _fadeDuration = 0.5f;
  [SerializeField] private float _fadeHoldDuration = 0.2f;

  [Header("Transition - Phase Label")]
  [SerializeField] private CanvasGroup _phaseLabelPanel;
  [SerializeField] private TMP_Text _phaseLabelText;
  [SerializeField] private float _phaseFadeDuration = 0.3f;
  [SerializeField] private float _phaseHoldDuration = 1.5f;

  public event Action OnSkipTurn;

  private void Start()
  {
    nextTurnButton.onClick.AddListener(() => OnSkipTurn?.Invoke());

    _fadeOverlay.alpha = 0f;
    _fadeOverlay.gameObject.SetActive(false);

    _phaseLabelPanel.alpha = 0f;
    _phaseLabelPanel.gameObject.SetActive(false);
  }

  public void SetTurnView(int day, string turnName)
  {
    dayText.text = $"Day {day}";
    turnStateText.text = turnName;
  }

  public void ShowSkipButton() => nextTurnButton.gameObject.SetActive(true);
  public void HideSkipButton() => nextTurnButton.gameObject.SetActive(false);

  public void PlayTurnTransition(string label, Action onMidpoint, Action onComplete)
  {
    StartCoroutine(TransitionRoutine(label, onMidpoint, onComplete));
  }

  private IEnumerator TransitionRoutine(string label, Action onMidpoint, Action onComplete)
  {
    _fadeOverlay.gameObject.SetActive(true);

    yield return Fade(_fadeOverlay, 1f, _fadeDuration);

    onMidpoint?.Invoke();

    yield return new WaitForSeconds(_fadeHoldDuration);

    yield return Fade(_fadeOverlay, 0f, _fadeDuration);

    _fadeOverlay.gameObject.SetActive(false);

    _phaseLabelText.text = label;
    _phaseLabelPanel.gameObject.SetActive(true);

    yield return Fade(_phaseLabelPanel, 1f, _phaseFadeDuration);
    yield return new WaitForSeconds(_phaseHoldDuration);
    yield return Fade(_phaseLabelPanel, 0f, _phaseFadeDuration);

    _phaseLabelPanel.gameObject.SetActive(false);
    onComplete?.Invoke();
  }

  private IEnumerator Fade(CanvasGroup group, float target, float duration)
  {
    float start = group.alpha;
    float elapsed = 0f;
    while (elapsed < duration)
    {
      elapsed += Time.deltaTime;
      group.alpha = Mathf.Lerp(start, target, elapsed / duration);
      yield return null;
    }
    group.alpha = target;
  }
}
