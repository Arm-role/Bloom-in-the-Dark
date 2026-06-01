#nullable enable

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Book-style menu UI (flat page list — no tabs):
//   Page content (mid): title + sprite + description
//   Footer:             [← Prev] [page X/N] [Next →]
//
// Input: H toggle (เสมอ), Esc close (เฉพาะตอน menu โผล่)
// _menuRoot toggle on/off; root active เสมอเพื่อให้ Update ทำงาน
public sealed class HintMenuView : MonoBehaviour, IHintMenuView
{
  [Header("Root (toggle visibility)")]
  [SerializeField] private GameObject _menuRoot = null!;

  [Header("Page content")]
  [SerializeField] private TMP_Text _pageTitleText = null!;
  [SerializeField] private GameObject _spriteRoot = null!;
  [SerializeField] private Image _spriteImage = null!;

  [Header("Footer")]
  [SerializeField] private Button _prevPageButton = null!;
  [SerializeField] private Button _nextPageButton = null!;
  [SerializeField] private TMP_Text _pageIndicatorText = null!;

  [Header("Close")]
  [SerializeField] private Button _closeButton = null!;

  [Header("Input")]
  [SerializeField] private KeyCode _toggleKey = KeyCode.H;

  public event Action? OnToggleRequested;
  public event Action? OnCloseRequested;
  public event Action? OnPrevPageRequested;
  public event Action? OnNextPageRequested;

  private bool _isVisible;

  private void Awake()
  {
    _closeButton.onClick.AddListener(RaiseClose);
    _prevPageButton.onClick.AddListener(RaisePrev);
    _nextPageButton.onClick.AddListener(RaiseNext);

    _menuRoot.SetActive(false);
    _spriteRoot.SetActive(false);
  }

  private void OnDestroy()
  {
    _closeButton.onClick.RemoveListener(RaiseClose);
    _prevPageButton.onClick.RemoveListener(RaisePrev);
    _nextPageButton.onClick.RemoveListener(RaiseNext);
  }

  private void Update()
  {
    if (Input.GetKeyDown(_toggleKey))
      OnToggleRequested?.Invoke();

    if (_isVisible && Input.GetKeyDown(KeyCode.Escape))
      OnCloseRequested?.Invoke();
  }

  // ==========================
  // IHintMenuView
  // ==========================

  public void ShowMenu()
  {
    _menuRoot.SetActive(true);
    _isVisible = true;
  }

  public void ShowPage(IHintEntry entry, int pageIndex, int pageCount)
  {
    _pageTitleText.text = entry.Title;
    _pageIndicatorText.text = $"{pageIndex + 1} / {pageCount}";

    _prevPageButton.interactable = pageIndex > 0;
    _nextPageButton.interactable = pageIndex < pageCount - 1;

    if (entry.Media is ISpriteMedia sprite)
    {
      _spriteImage.sprite = sprite.Sprite;
      _spriteRoot.SetActive(true);
    }
    else
    {
      _spriteRoot.SetActive(false);
    }
  }

  public void Hide()
  {
    _menuRoot.SetActive(false);
    _isVisible = false;
  }

  // ==========================
  // Internal
  // ==========================

  private void RaiseClose() => OnCloseRequested?.Invoke();
  private void RaisePrev() => OnPrevPageRequested?.Invoke();
  private void RaiseNext() => OnNextPageRequested?.Invoke();
}
