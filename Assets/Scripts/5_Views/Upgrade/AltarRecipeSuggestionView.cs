using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltarRecipeSuggestionView : MonoBehaviour, IAltarRecipeSuggestionView
{
  [SerializeField] private Image[] _resultSlots;

  [Header("Float Animation")]
  [SerializeField] private float _bobAmplitude = 8f;
  [SerializeField] private float _bobSpeed = 2f;
  [SerializeField] private float _phaseOffset = 0.4f;

  private IItemIconProvider _iconProvider;
  private Vector2[] _slotBasePositions;

  private void Awake()
  {
    _slotBasePositions = new Vector2[_resultSlots.Length];
    for (int i = 0; i < _resultSlots.Length; i++)
      _slotBasePositions[i] = _resultSlots[i].rectTransform.anchoredPosition;
  }

  private void Update()
  {
    for (int i = 0; i < _resultSlots.Length; i++)
    {
      var slot = _resultSlots[i];
      // slot อาจถูก destroy ตอน scene unload (GameOver) ขณะ view ยัง tick ค้าง
      if (slot == null || !slot.enabled) continue;
      float bob = Mathf.Sin(Time.time * _bobSpeed + i * _phaseOffset) * _bobAmplitude;
      slot.rectTransform.anchoredPosition = _slotBasePositions[i] + Vector2.up * bob;
    }
  }

  public void Initialize(IItemIconProvider iconProvider)
  {
    _iconProvider = iconProvider;
    HideAll();
  }

  public void ShowSuggestions(List<AltarRecipeDefinition> recipes, int startSlot = 0)
  {
    for (int i = 0; i < _resultSlots.Length; i++)
    {
      int recipeIndex = i - startSlot;
      if (i < startSlot || recipeIndex >= recipes.Count)
      {
        _resultSlots[i].enabled = false;
      }
      else
      {
        _resultSlots[i].sprite = recipes[recipeIndex].GetPreviewIcon(_iconProvider);
        _resultSlots[i].enabled = true;
      }
    }
  }

  public void HideAll()
  {
    foreach (var slot in _resultSlots)
      slot.enabled = false;
  }
}
