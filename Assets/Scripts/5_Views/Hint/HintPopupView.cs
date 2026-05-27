#nullable enable

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// MonoBehaviour modal — display title + body + optional media + close affordances
// Close trigger: Close button + background dim click + Esc (ทุก trigger → OnCloseRequested event)
// Layout: full-screen overlay → dim background → center panel (Title/Body/Image/Close)
public sealed class HintPopupView : MonoBehaviour, IHintPopupView
{
  [Header("Root")]
  [SerializeField] private GameObject _root = null!;

  [Header("Background (click ที่นี่ → close)")]
  [SerializeField] private Button _backgroundDimButton = null!;

  [Header("Content")]
  [SerializeField] private TMP_Text _titleText = null!;
  [SerializeField] private TMP_Text _descriptionText = null!;

  [Header("Media (ซ่อนถ้า entry.Media == null)")]
  [SerializeField] private GameObject _mediaRoot = null!;
  [SerializeField] private Image _mediaImage = null!;

  [Header("Close")]
  [SerializeField] private Button _closeButton = null!;

  public event Action? OnCloseRequested;

  private bool _isVisible;

  private void Awake()
  {
    _closeButton.onClick.AddListener(RaiseClose);
    _backgroundDimButton.onClick.AddListener(RaiseClose);
    _root.SetActive(false);
  }

  private void OnDestroy()
  {
    _closeButton.onClick.RemoveListener(RaiseClose);
    _backgroundDimButton.onClick.RemoveListener(RaiseClose);
  }

  // Esc → close (game ถูก pause อยู่ ตอน popup โผล่ → unscaled input ปกติยังใช้ Input.GetKeyDown ได้)
  private void Update()
  {
    if (!_isVisible) return;
    if (Input.GetKeyDown(KeyCode.Escape))
      RaiseClose();
  }

  public void Show(IHintEntry entry)
  {
#if UNITY_EDITOR
    Debug.Log($"[HintPopupView] Show title='{entry.Title}' root='{_root?.name ?? "NULL"}'");
#endif
    _titleText.text = entry.Title;
    _descriptionText.text = entry.Description;

    if (entry.Media != null)
    {
      _mediaImage.sprite = entry.Media;
      _mediaRoot.SetActive(true);
    }
    else
    {
      _mediaRoot.SetActive(false);
    }

    _root.SetActive(true);
    _isVisible = true;
  }

  public void Hide()
  {
    _root.SetActive(false);
    _isVisible = false;
  }

  private void RaiseClose() => OnCloseRequested?.Invoke();
}
