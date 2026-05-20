using System;

public interface IPauseMenuView
{
  event Action OnResumeClicked;
  event Action OnOptionClicked;
  event Action OnExitToMainMenuClicked;
  event Action OnExitGameClicked;

  void Show();
  void Hide();
}
