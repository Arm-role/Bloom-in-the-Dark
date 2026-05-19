using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryboardView : MonoBehaviour, IStoryboardView
{
  [SerializeField] private Image _panelImage;
  [SerializeField] private TMP_Text _dialogueText;
  [SerializeField] private CanvasGroup _canvasGroup;

  public event Action OnAdvanceRequested;
  public bool IsTyping => _isTyping;

  private bool _isTyping;
  private Coroutine _typeCoroutine;

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
      OnAdvanceRequested?.Invoke();
  }

  public void ShowPanel(Sprite image, string text, float charsPerSecond)
  {
    if (_panelImage != null)
    {
      _panelImage.sprite = image;
      _panelImage.enabled = image != null;
    }

    if (_typeCoroutine != null)
      StopCoroutine(_typeCoroutine);

    _typeCoroutine = StartCoroutine(TypeText(text, charsPerSecond));
  }

  public void CompleteText()
  {
    _isTyping = false;
  }

  public IEnumerator FadeIn(float duration)
  {
    _canvasGroup.alpha = 0f;
    float t = 0f;
    while (t < duration)
    {
      t += Time.unscaledDeltaTime;
      _canvasGroup.alpha = Mathf.Clamp01(t / duration);
      yield return null;
    }
    _canvasGroup.alpha = 1f;
  }

  public IEnumerator FadeOut(float duration)
  {
    _canvasGroup.alpha = 1f;
    float t = 0f;
    while (t < duration)
    {
      t += Time.unscaledDeltaTime;
      _canvasGroup.alpha = 1f - Mathf.Clamp01(t / duration);
      yield return null;
    }
    _canvasGroup.alpha = 0f;
  }

  private IEnumerator TypeText(string text, float charsPerSecond)
  {
    _isTyping = true;
    _dialogueText.text = "";

    float delay = 1f / Mathf.Max(charsPerSecond, 1f);

    foreach (char c in text)
    {
      if (!_isTyping) break; // CompleteText() ถูกเรียก
      _dialogueText.text += c;
      yield return new WaitForSecondsRealtime(delay);
    }

    _dialogueText.text = text;
    _isTyping = false;
  }
}
