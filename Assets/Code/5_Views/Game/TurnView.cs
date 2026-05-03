using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnView : MonoBehaviour, ITurnView
{
  [Header("UI")]
  [SerializeField] private TMP_Text dayText;
  [SerializeField] private TMP_Text turnStateText;
  [SerializeField] private Button nextTurnButton;

  public event Action OnSkipTurn;

  private void Start()
  {
    nextTurnButton.onClick.AddListener(() => { OnSkipTurn?.Invoke(); });
  }

  public void SetTurnView(int day, string turnName)
  {
    dayText.text = $"Day {day}";
    turnStateText.text = $"Phase {turnName}";
  }

  public void ShowSkipButton()
  {
    nextTurnButton.gameObject.SetActive(true);
  }
  public void HideSkipButton()
  {
    nextTurnButton.gameObject.SetActive(false);
  }
}
