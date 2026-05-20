using TMPro;
using UnityEngine;

public class RespawnView : MonoBehaviour, IRespawnView
{
  [SerializeField] private GameObject _root;
  [SerializeField] private TMP_Text _countdownText;
  [SerializeField] private string _countdownFormat = "Respawning in {0}";

  private void Awake()
  {
    if (_root != null) _root.SetActive(false);
  }

  public void Show()
  {
    if (_root != null) _root.SetActive(true);
  }

  public void Hide()
  {
    if (_root != null) _root.SetActive(false);
  }

  public void SetCountdown(float remainingSeconds)
  {
    if (_countdownText == null) return;
    int seconds = Mathf.CeilToInt(Mathf.Max(0f, remainingSeconds));
    _countdownText.text = string.Format(_countdownFormat, seconds);
  }
}
