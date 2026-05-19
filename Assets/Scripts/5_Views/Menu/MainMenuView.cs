using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour, IMainMenuView
{
  [SerializeField] private Button _newGameButton;
  [SerializeField] private Button _optionButton;
  [SerializeField] private Button _exitButton;

  public event Action OnNewGameClicked;
  public event Action OnOptionClicked;
  public event Action OnExitClicked;

  private void Awake()
  {
    _newGameButton.onClick.AddListener(() => OnNewGameClicked?.Invoke());
    _optionButton.onClick.AddListener(() => OnOptionClicked?.Invoke());
    _exitButton.onClick.AddListener(() => OnExitClicked?.Invoke());
  }

  private void OnDestroy()
  {
    _newGameButton.onClick.RemoveAllListeners();
    _optionButton.onClick.RemoveAllListeners();
    _exitButton.onClick.RemoveAllListeners();
  }
}
