using UnityEngine;
using UnityEngine.SceneManagement;

// Pause menu — implements IModalUI:
//   Esc → OnDismiss → ModalUIStack.RouteDismiss → HandleDismiss
//   PausesGame=true → stack จัดการ Time.timeScale ผ่าน OnFirstPausingPush/OnLastPausingPop
//
// State machine: ยัง ChangeState(Pause/Gameplay) ระหว่าง migration เผื่อ system อื่นที่ check state
// (Inventory/Upgrade/Trade ยังใช้ state machine pattern จนกว่า Phase 5)
public class PauseMenuController : MonoBehaviour, IModalUI
{
  [SerializeField] private MonoBehaviour _viewBehaviour;
  [SerializeField] private OptionController _optionController;
  [SerializeField] private string _mainMenuSceneName = "MainMenu";

  private IPauseMenuView _view;
  private GameStateMachine _stateMachine;
  private ModalUIStack _modalStack;
  private bool _isOpen;

  public bool IsOpen => _isOpen;
  public bool PausesGame => true;

  // Esc ใน Pause: ถ้า Option panel เปิดทับ → ปิด option ก่อน (ไม่ resume); ไม่งั้น Resume
  public void HandleDismiss()
  {
    if (_optionController != null && _optionController.IsShown)
    {
      _optionController.Hide();
      return;
    }
    Resume();
  }

  public void Initialize(GameStateMachine stateMachine, ModalUIStack modalStack)
  {
    _stateMachine = stateMachine;
    _modalStack = modalStack;

    _view = _viewBehaviour as IPauseMenuView;
    if (_view == null)
    {
      Debug.LogWarning("[PauseMenuController] _viewBehaviour does not implement IPauseMenuView");
      return;
    }

    _view.OnResumeClicked += Resume;
    _view.OnOptionClicked += HandleOption;
    _view.OnExitToMainMenuClicked += HandleExitToMainMenu;
    _view.OnExitGameClicked += HandleExitGame;

    _view.Hide();
  }

  private void OnDestroy()
  {
    if (_view == null) return;
    _view.OnResumeClicked -= Resume;
    _view.OnOptionClicked -= HandleOption;
    _view.OnExitToMainMenuClicked -= HandleExitToMainMenu;
    _view.OnExitGameClicked -= HandleExitGame;
  }

  // เปิด Pause — เรียกจาก installer OnDismiss handler ตอน stack ว่าง + state=Gameplay
  public void Open()
  {
    if (_stateMachine == null || _isOpen) return;
    if (_stateMachine.CurrentState != EGameState.Gameplay) return;

    _isOpen = true;
    _modalStack.Push(this);  // stack ตั้ง Time.timeScale=0 ผ่าน OnFirstPausingPush
    _stateMachine.ChangeState(EGameState.Pause);
    _view.Show();
  }

  private void Resume()
  {
    if (!_isOpen) return;

    _isOpen = false;
    _view.Hide();
    _modalStack.Pop(this);  // stack restore Time.timeScale=1 ผ่าน OnLastPausingPop
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
    // safety net: scene load ต้องใช้ timeScale=1
    // pop stack ก่อนเปลี่ยน scene กัน state stack รั่ว
    if (_isOpen)
    {
      _modalStack.Pop(this);
      _isOpen = false;
    }
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
