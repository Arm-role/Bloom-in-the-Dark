using System;
using UnityEngine;
using UnityEngine.UI;

public class EndGameView : MonoBehaviour, IEndGameView
{
  [SerializeField] private Button _endlessButton;
  [SerializeField] private Button _mainMenuButton;

  public event Action OnEndlessClicked;
  public event Action OnMainMenuClicked;

  private void Awake()
  {
    _endlessButton.onClick.AddListener(() => OnEndlessClicked?.Invoke());
    _mainMenuButton.onClick.AddListener(() => OnMainMenuClicked?.Invoke());
  }

  private void OnDestroy()
  {
    _endlessButton.onClick.RemoveAllListeners();
    _mainMenuButton.onClick.RemoveAllListeners();
  }
}
