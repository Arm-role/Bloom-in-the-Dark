using TMPro;
using UnityEngine;

public class TurnView : MonoBehaviour, ITurnView
{
  [SerializeField] private TextMeshProUGUI turnStateText;
  public void SetTurnView(string text)
  {
    turnStateText.text = text;
  }
}
