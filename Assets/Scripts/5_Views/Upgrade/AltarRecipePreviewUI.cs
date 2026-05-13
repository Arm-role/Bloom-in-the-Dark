using System;
using UnityEngine;
using UnityEngine.UI;

public class AltarRecipePreviewUI : MonoBehaviour
{
  [SerializeField] private GameObject _panel;
  [SerializeField] private Image _resultIcon;
  [SerializeField] private Button _confirmButton;

  private IItemIconProvider _iconProvider;
  private Action _onConfirm;

  public void Initialize(IItemIconProvider iconProvider)
  {
    _iconProvider = iconProvider;
    _confirmButton.onClick.AddListener(OnConfirmClicked);
    Hide();
  }

  public void Show(AltarRecipeDefinition recipe, Action onConfirm)
  {
    _onConfirm = onConfirm;
    _resultIcon.sprite = _iconProvider.GetIcon(recipe.ResultItemId);
    _panel.SetActive(true);
  }

  public void Hide()
  {
    _panel.SetActive(false);
    _onConfirm = null;
  }

  private void OnConfirmClicked()
  {
    _onConfirm?.Invoke();
  }
}
