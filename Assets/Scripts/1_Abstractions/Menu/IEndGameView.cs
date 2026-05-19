using System;

public interface IEndGameView
{
  event Action OnEndlessClicked;
  event Action OnMainMenuClicked;
}
