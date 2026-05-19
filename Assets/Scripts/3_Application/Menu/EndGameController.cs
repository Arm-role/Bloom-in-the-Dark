using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
  [Header("Root")]
  [SerializeField] private GameObject _panelRoot;
  [SerializeField] private float _fadeDuration = 0.5f;

  [Header("Storyboard Phase")]
  [SerializeField] private GameObject _storyboardPanelRoot;
  [SerializeField] private MonoBehaviour _storyboardViewBehaviour;
  [SerializeField] private StoryboardSO _storyboard;

  [Header("Thanks Phase")]
  [SerializeField] private GameObject _thanksPanel;
  [SerializeField] private MonoBehaviour _endGameViewBehaviour;
  [SerializeField] private string _mainMenuSceneName = "MainMenu";

  private IStoryboardView _storyboardView;
  private IEndGameView _endGameView;

  private int _currentPanel = -1;
  private bool _shown;
  private bool _inputReady;

  private void Awake()
  {
    _storyboardView = _storyboardViewBehaviour as IStoryboardView;
    _endGameView = _endGameViewBehaviour as IEndGameView;

    if (_storyboardView == null)
      Debug.LogWarning("[EndGameController] _storyboardViewBehaviour does not implement IStoryboardView");

    if (_endGameView == null)
      Debug.LogWarning("[EndGameController] _endGameViewBehaviour does not implement IEndGameView");

    if (_storyboardView != null)
      _storyboardView.OnAdvanceRequested += HandleAdvance;

    if (_endGameView != null)
    {
      _endGameView.OnEndlessClicked += HandleEndless;
      _endGameView.OnMainMenuClicked += HandleMainMenu;
    }

    if (_panelRoot != null) _panelRoot.SetActive(false);
    if (_thanksPanel != null) _thanksPanel.SetActive(false);
  }

  private void OnDestroy()
  {
    if (_storyboardView != null)
      _storyboardView.OnAdvanceRequested -= HandleAdvance;

    if (_endGameView != null)
    {
      _endGameView.OnEndlessClicked -= HandleEndless;
      _endGameView.OnMainMenuClicked -= HandleMainMenu;
    }
  }

  public void Show()
  {
    if (_shown) return;
    _shown = true;
    _inputReady = false;

    if (_panelRoot != null) _panelRoot.SetActive(true);
    if (_storyboardPanelRoot != null) _storyboardPanelRoot.SetActive(true);
    if (_thanksPanel != null) _thanksPanel.SetActive(false);

    StartCoroutine(BeginFlow());
  }

  private IEnumerator BeginFlow()
  {
    if (_storyboardView != null)
      yield return _storyboardView.FadeIn(_fadeDuration);

    ShowNextPanel();

    // grace period — กัน input phantom (click จาก auto-attack ตอนตี boss)
    yield return new WaitForSecondsRealtime(1f);
    _inputReady = true;
  }

  private void HandleAdvance()
  {
    if (_storyboardView == null) return;
    if (!_inputReady) return; // กัน input phantom ระหว่าง fade in / first panel

    // กด 1: ถ้า text กำลังพิมพ์ → snap แสดงเต็มทันที
    if (_storyboardView.IsTyping)
    {
      _storyboardView.CompleteText();
      return;
    }

    // กดถัดไป: ไป panel ต่อไป หรือถ้าหมดแล้ว → thanks panel
    bool hasNext = _storyboard != null
                && _storyboard.panels != null
                && _currentPanel + 1 < _storyboard.panels.Length;

    if (hasNext)
      ShowNextPanel();
    else
      ShowThanksPanel();
  }

  private void ShowNextPanel()
  {
    if (_storyboard == null || _storyboard.panels == null)
    {
      ShowThanksPanel();
      return;
    }

    int nextIndex = _currentPanel + 1;

    // ถ้าเลย length → ไป thanks (case: ไม่มี panel เลย หรือ panel หมดแล้ว)
    if (nextIndex >= _storyboard.panels.Length)
    {
      ShowThanksPanel();
      return;
    }

    _currentPanel = nextIndex;
    var panel = _storyboard.panels[_currentPanel];
    _storyboardView.ShowPanel(panel.image, panel.text, panel.charsPerSecond);
  }

  private void ShowThanksPanel()
  {
    if (_storyboardView != null)
      _storyboardView.OnAdvanceRequested -= HandleAdvance;

    if (_storyboardPanelRoot != null) _storyboardPanelRoot.SetActive(false);
    if (_thanksPanel != null) _thanksPanel.SetActive(true);
  }

  private void HandleEndless()
  {
    // resume gameplay — IsEndlessMode ถูก set ตั้งแต่ BossDefeatHandler แล้ว
    HideAll();
  }

  private void HideAll()
  {
    if (_thanksPanel != null) _thanksPanel.SetActive(false);
    if (_storyboardPanelRoot != null) _storyboardPanelRoot.SetActive(false);
    if (_panelRoot != null) _panelRoot.SetActive(false);
  }

  private void HandleMainMenu()
  {
    if (string.IsNullOrEmpty(_mainMenuSceneName))
    {
      Debug.LogWarning("[EndGameController] _mainMenuSceneName is empty");
      return;
    }
    SceneManager.LoadScene(_mainMenuSceneName);
  }
}
