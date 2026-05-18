using System;

public interface ITurnView
{
  event Action OnSkipTurn;
  void SetTurnView(int day, string turnName);
  void ShowSkipButton();
  void HideSkipButton();
  void PlayTurnTransition(string label, Action onMidpoint, Action onComplete);
}