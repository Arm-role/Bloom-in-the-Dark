using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
  [SerializeField] private MonoBehaviour _viewBehaviour;
  [SerializeField] private GameObject _panelRoot;
  [SerializeField] private string _mainMenuSceneName = "MainMenu";

  private IEndGameView _view;

  private void Awake()
  {
    _view = _viewBehaviour as IEndGameView;
    if (_view == null)
    {
      Debug.LogWarning("[EndGameController] _viewBehaviour does not implement IEndGameView");
      return;
    }

    _view.OnEndlessClicked += HandleEndless;
    _view.OnMainMenuClicked += HandleMainMenu;

    if (_panelRoot != null) _panelRoot.SetActive(false);
  }

  private void OnDestroy()
  {
    if (_view == null) return;
    _view.OnEndlessClicked -= HandleEndless;
    _view.OnMainMenuClicked -= HandleMainMenu;
  }

  public void Show()
  {
    if (_panelRoot != null) _panelRoot.SetActive(true);
    Time.timeScale = 0f;
  }

  public void Hide()
  {
    Time.timeScale = 1f;
    if (_panelRoot != null) _panelRoot.SetActive(false);
  }

  private void HandleEndless()
  {
    // resume battle ต่อจากเดิม — IsEndlessMode ถูก set ตั้งแต่ BossDefeatHandler แล้ว
    Hide();
  }

  private void HandleMainMenu()
  {
    Time.timeScale = 1f;

    if (string.IsNullOrEmpty(_mainMenuSceneName))
    {
      Debug.LogWarning("[EndGameController] _mainMenuSceneName is empty");
      return;
    }
    SceneManager.LoadScene(_mainMenuSceneName);
  }
}
