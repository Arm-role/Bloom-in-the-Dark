#nullable enable

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// MonoBehaviour modal — display title + body + sprite + close affordances
// Media dispatch: pattern match ITutorialMedia → ISpriteMedia → Image (video รองรับใน future)
public sealed class HintPopupView : MonoBehaviour, IHintPopupView
{
  [Header("Root")]
  [SerializeField] private GameObject _root = null!;

  [Header("Background (click ที่นี่ → close)")]
  [SerializeField] private Button _backgroundDimButton = null!;

  [Header("Content")]
  [SerializeField] private TMP_Text _titleText = null!;

  [Header("Media — Sprite")]
  [SerializeField] private GameObject _spriteRoot = null!;
  [SerializeField] private Image _spriteImage = null!;

  [Header("Close")]
  [SerializeField] private Button _closeButton = null!;

  public event Action? OnCloseRequested;

  private bool _isVisible;

  private void Awake()
  {
    _closeButton.onClick.AddListener(RaiseClose);
    _backgroundDimButton.onClick.AddListener(RaiseClose);
    _root.SetActive(false);
    _spriteRoot.SetActive(false);
  }

  private void OnDestroy()
  {
    _closeButton.onClick.RemoveListener(RaiseClose);
    _backgroundDimButton.onClick.RemoveListener(RaiseClose);
  }

  private void Update()
  {
    if (!_isVisible) return;
    if (Input.GetKeyDown(KeyCode.Escape))
      RaiseClose();
  }

  public void Show(IHintEntry entry)
  {
#if UNITY_EDITOR
    Debug.Log($"[HintPopupView] Show title='{entry.Title}' root='{_root?.name ?? "NULL"}' mediaType={entry.Media?.GetType().Name ?? "null"}");
#endif
    _titleText.text = entry.Title;

    DispatchMedia(entry.Media);

    _root.SetActive(true);
    _isVisible = true;
  }

  public void Hide()
  {
    _spriteRoot.SetActive(false);
    _root.SetActive(false);
    _isVisible = false;
  }

  // ============================
  // Media dispatch
  // ============================

  private void DispatchMedia(ITutorialMedia? media)
  {
    if (media is ISpriteMedia sprite)
    {
      _spriteImage.sprite = sprite.Sprite;
      _spriteRoot.SetActive(true);
    }
    else
    {
      _spriteRoot.SetActive(false);
    }
  }

  private void RaiseClose() => OnCloseRequested?.Invoke();
}
