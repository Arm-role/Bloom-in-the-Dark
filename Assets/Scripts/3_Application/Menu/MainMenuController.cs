using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
  [SerializeField] private MonoBehaviour _viewBehaviour;
  [SerializeField] private OptionController _optionController;
  [SerializeField] private string _nextSceneName;

  private IMainMenuView _view;

  private void Start()
  {
    _view = _viewBehaviour as IMainMenuView;
    if (_view == null)
    {
      Debug.LogWarning("[MainMenuController] _viewBehaviour does not implement IMainMenuView");
      return;
    }

    _view.OnNewGameClicked += HandleNewGame;
    _view.OnOptionClicked += HandleOption;
    _view.OnExitClicked += HandleExit;
  }

  private void OnDestroy()
  {
    if (_view == null) return;
    _view.OnNewGameClicked -= HandleNewGame;
    _view.OnOptionClicked -= HandleOption;
    _view.OnExitClicked -= HandleExit;
  }

  private void HandleNewGame()
  {
    if (string.IsNullOrEmpty(_nextSceneName))
    {
      Debug.LogWarning("[MainMenuController] _nextSceneName is empty");
      return;
    }

    GameSession.Reset();
    SceneManager.LoadScene(_nextSceneName);
  }

  private void HandleOption() => _optionController?.Show();

  private void HandleExit()
  {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }
}
