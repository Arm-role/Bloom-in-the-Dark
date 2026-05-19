using System;

public interface IMainMenuView
{
  event Action OnNewGameClicked;
  event Action OnOptionClicked;
  event Action OnExitClicked;
}
