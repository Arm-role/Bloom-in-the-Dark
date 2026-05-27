#nullable enable

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// MonoBehaviour menu — scroll list ของ entry button ที่ instantiate จาก prefab
// Input: กด H toggle (เสมอ — เปิดหรือปิด), Esc close (ขณะ menu โผล่)
// _menuRoot ถูก toggle on/off; root GameObject ของ component นี้ active เสมอเพื่อให้ Update ทำงาน
public sealed class HintMenuView : MonoBehaviour, IHintMenuView
{
  [Header("Root (toggle visibility)")]
  [SerializeField] private GameObject _menuRoot = null!;

  [Header("Entry list")]
  [SerializeField] private Transform _entryListParent = null!;
  [SerializeField] private GameObject _entryButtonPrefab = null!;   // prefab มี Button + TMP_Text child + Image (optional thumbnail)

  [Header("Close")]
  [SerializeField] private Button _closeButton = null!;

  [Header("Input")]
  [SerializeField] private KeyCode _toggleKey = KeyCode.H;

  public event Action? OnToggleRequested;
  public event Action? OnCloseRequested;
  public event Action<string>? OnEntryClicked;

  private readonly List<GameObject> _spawnedTiles = new();
  private bool _isVisible;

  private void Awake()
  {
    _closeButton.onClick.AddListener(RaiseClose);
    _menuRoot.SetActive(false);
  }

  private void OnDestroy()
  {
    _closeButton.onClick.RemoveListener(RaiseClose);
    ClearTiles();
  }

  // toggle key ทำงานเสมอ (ขณะ menu ปิดด้วย) — Update บน root GameObject ที่ active
  // Esc ทำงานเฉพาะตอน menu โผล่
  private void Update()
  {
    if (Input.GetKeyDown(_toggleKey))
      OnToggleRequested?.Invoke();

    if (_isVisible && Input.GetKeyDown(KeyCode.Escape))
      OnCloseRequested?.Invoke();
  }

  public void ShowMenu(IReadOnlyList<IHintEntry> entries)
  {
    BuildTiles(entries);
    _menuRoot.SetActive(true);
    _isVisible = true;
  }

  public void Hide()
  {
    _menuRoot.SetActive(false);
    _isVisible = false;
  }

  // ==========================
  // Internal
  // ==========================

  private void BuildTiles(IReadOnlyList<IHintEntry> entries)
  {
    ClearTiles();

    foreach (var entry in entries)
    {
      var tile = Instantiate(_entryButtonPrefab, _entryListParent);
      _spawnedTiles.Add(tile);

      // ตั้ง label — หา TMP_Text ใน prefab (child แรกหรือชั้นบนสุด)
      var label = tile.GetComponentInChildren<TMP_Text>();
      if (label != null)
        label.text = entry.Title;

      // ผูก click → ส่ง id ของ entry ออก event
      var button = tile.GetComponent<Button>();
      if (button != null)
      {
        var idCapture = entry.Id;   // capture local — กัน closure share ID ตัวสุดท้ายของ loop
        button.onClick.AddListener(() => OnEntryClicked?.Invoke(idCapture));
      }
    }
  }

  private void ClearTiles()
  {
    foreach (var tile in _spawnedTiles)
      if (tile != null)
        Destroy(tile);
    _spawnedTiles.Clear();
  }

  private void RaiseClose() => OnCloseRequested?.Invoke();
}
