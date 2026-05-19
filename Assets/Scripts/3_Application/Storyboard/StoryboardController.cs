using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryboardController : MonoBehaviour
{
  [SerializeField] private StoryboardSO _storyboard;
  [SerializeField] private MonoBehaviour _viewBehaviour;
  [SerializeField] private float _fadeDuration = 0.5f;
  [SerializeField] private string _nextSceneName;

  public event Action OnComplete;

  private IStoryboardView _view;
  private int _currentPanel = -1;
  private bool _transitioning;

  private AsyncOperation _preloadOp;

  private void Start()
  {
    _view = _viewBehaviour as IStoryboardView;
    if (_view == null)
    {
      Debug.LogWarning("[StoryboardController] _viewBehaviour does not implement IStoryboardView");
      return;
    }

    StartPreloadNextScene();

    _view.OnAdvanceRequested += HandleAdvance;
    StartCoroutine(BeginStoryboard());
  }

  private void OnDestroy()
  {
    if (_view != null)
      _view.OnAdvanceRequested -= HandleAdvance;
  }

  private void StartPreloadNextScene()
  {
    if (string.IsNullOrEmpty(_nextSceneName)) return;

    _preloadOp = SceneManager.LoadSceneAsync(_nextSceneName);
    _preloadOp.allowSceneActivation = false;
  }

  private IEnumerator BeginStoryboard()
  {
    yield return _view.FadeIn(_fadeDuration);
    ShowNextPanel();
  }

  private void HandleAdvance()
  {
    if (_transitioning) return;

    // กด 1 ครั้ง: ถ้า text ยังพิมพ์อยู่ → โชว์ทันที
    if (_view.IsTyping)
    {
      _view.CompleteText();
      return;
    }

    // กด 2 ครั้ง: ไป panel ถัดไป
    bool hasNext = _currentPanel + 1 < _storyboard.panels.Length;
    if (hasNext)
      ShowNextPanel();
    else
      StartCoroutine(FinishStoryboard());
  }

  private void ShowNextPanel()
  {
    _currentPanel++;
    var panel = _storyboard.panels[_currentPanel];
    _view.ShowPanel(panel.image, panel.text, panel.charsPerSecond);
  }

  private IEnumerator FinishStoryboard()
  {
    _transitioning = true;
    _view.OnAdvanceRequested -= HandleAdvance;
    yield return _view.FadeOut(_fadeDuration);
    OnComplete?.Invoke();

    if (_preloadOp != null)
    {
      // รอ scene โหลดเสร็จ (Unity ถือว่า 0.9 = พร้อม activate)
      while (_preloadOp.progress < 0.9f)
        yield return null;

      _preloadOp.allowSceneActivation = true;
    }
  }
}
