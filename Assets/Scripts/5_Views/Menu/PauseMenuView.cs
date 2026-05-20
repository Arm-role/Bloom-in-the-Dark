using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuView : MonoBehaviour, IPauseMenuView
{
  [SerializeField] private GameObject _panelRoot;
  [SerializeField] private Button _resumeButton;
  [SerializeField] private Button _optionButton;
  [SerializeField] private Button _exitToMainMenuButton;
  [SerializeField] private Button _exitGameButton;

  public event Action OnResumeClicked;
  public event Action OnOptionClicked;
  public event Action OnExitToMainMenuClicked;
  public event Action OnExitGameClicked;

  private void Awake()
  {
    _resumeButton.onClick.AddListener(() => OnResumeClicked?.Invoke());
    _optionButton.onClick.AddListener(() => OnOptionClicked?.Invoke());
    _exitToMainMenuButton.onClick.AddListener(() => OnExitToMainMenuClicked?.Invoke());
    _exitGameButton.onClick.AddListener(() => OnExitGameClicked?.Invoke());
  }

  private void OnDestroy()
  {
    _resumeButton.onClick.RemoveAllListeners();
    _optionButton.onClick.RemoveAllListeners();
    _exitToMainMenuButton.onClick.RemoveAllListeners();
    _exitGameButton.onClick.RemoveAllListeners();
  }

  public void Show() => _panelRoot.SetActive(true);
  public void Hide() => _panelRoot.SetActive(false);
}
