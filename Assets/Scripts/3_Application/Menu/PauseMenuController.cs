using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
  [SerializeField] private MonoBehaviour _viewBehaviour;
  [SerializeField] private OptionController _optionController;
  [SerializeField] private string _mainMenuSceneName = "MainMenu";

  private IPauseMenuView _view;
  private GameStateMachine _stateMachine;
  private IPlayerInput _input;

  public void Initialize(GameStateMachine stateMachine, IPlayerInput input)
  {
    _stateMachine = stateMachine;
    _input = input;

    _view = _viewBehaviour as IPauseMenuView;
    if (_view == null)
    {
      Debug.LogWarning("[PauseMenuController] _viewBehaviour does not implement IPauseMenuView");
      return;
    }

    _input.OnPauseToggle += HandlePauseToggle;
    _view.OnResumeClicked += Resume;
    _view.OnOptionClicked += HandleOption;
    _view.OnExitToMainMenuClicked += HandleExitToMainMenu;
    _view.OnExitGameClicked += HandleExitGame;

    _view.Hide();
  }

  private void OnDestroy()
  {
    if (_input != null)
      _input.OnPauseToggle -= HandlePauseToggle;

    if (_view == null) return;
    _view.OnResumeClicked -= Resume;
    _view.OnOptionClicked -= HandleOption;
    _view.OnExitToMainMenuClicked -= HandleExitToMainMenu;
    _view.OnExitGameClicked -= HandleExitGame;
  }

  // ESC toggle — pause ได้เฉพาะตอนอยู่ Gameplay เท่านั้น
  // กันไม่ให้ ESC ไปแย่ง state ตอน Inventory/Upgrade เปิดอยู่
  private void HandlePauseToggle()
  {
    if (_stateMachine == null) return;

    if (_stateMachine.CurrentState == EGameState.Gameplay)
    {
      Pause();
      return;
    }

    if (_stateMachine.CurrentState != EGameState.Pause) return;

    // ถ้า Option panel เปิดค้างอยู่ ESC ปิดแค่ Option ก่อน — ไม่ resume เกมทันที
    if (_optionController != null && _optionController.IsShown)
    {
      _optionController.Hide();
      return;
    }

    Resume();
  }

  private void Pause()
  {
    Time.timeScale = 0f;
    _stateMachine.ChangeState(EGameState.Pause);
    _view.Show();
  }

  private void Resume()
  {
    if (_stateMachine == null || _stateMachine.CurrentState != EGameState.Pause) return;

    _view.Hide();
    Time.timeScale = 1f;
    _stateMachine.ChangeState(EGameState.Gameplay);
  }

  private void HandleOption() => _optionController?.Show();

  private void HandleExitToMainMenu()
  {
    if (string.IsNullOrEmpty(_mainMenuSceneName))
    {
      Debug.LogWarning("[PauseMenuController] _mainMenuSceneName is empty");
      return;
    }
    // คืน timeScale ก่อนเปลี่ยน scene — ไม่งั้น MainMenu จะค้างเพราะ timeScale=0
    Time.timeScale = 1f;
    SceneManager.LoadScene(_mainMenuSceneName);
  }

  private void HandleExitGame()
  {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }
}
