using System.Collections;
using UnityEngine;

public class AltarController : MonoBehaviour
{
  [SerializeField] private GameTagAsset _moonBloomTag;
  [SerializeField] private GameTagAsset _plantTag;
  [SerializeField] private int[] _plantRequirementsPerLevel = { 1 };
  [SerializeField] private OfferingAltarController[] _offeringAltars;

  private AltarDomain _domain;
  private IUpgradeManagerView _managerView;
  private IProgressionView _progressionView;
  private IAltarRecipeSuggestionView _suggestionView;
  private PlantProgressionDomain _plantProgressionDomain;
  private PlayerProgression _playerProgression;
  private ItemFactory _itemFactory;
  private PlayerInventory _inventory;

  public void Initialize(
    IItemIconProvider iconProvider,
    AltarRecipeDatabase recipeDatabase,
    IUpgradeManagerView managerView,
    IProgressionView progressionView,
    IAltarRecipeSuggestionView suggestionView,
    PlayerProgression playerProgression,
    ItemFactory itemFactory,
    PlayerInventory inventory)
  {
    _managerView = managerView;
    _progressionView = progressionView;
    _suggestionView = suggestionView;
    _playerProgression = playerProgression;
    _itemFactory = itemFactory;
    _inventory = inventory;

    _plantProgressionDomain = new PlantProgressionDomain(progressionView);

    _domain = new AltarDomain(
      recipeDatabase,
      _moonBloomTag.RuntimeTag,
      _plantTag.RuntimeTag,
      GetRequiredCount);

    _domain.OnUpgradeReady             += HandleUpgradeReady;
    _domain.OnRecipeSuggestionsChanged += HandleSuggestionsChanged;
    _domain.OnCraftPreviewReady        += HandleCraftPreview;
    _domain.OnCraftReady               += HandleCraftReady;
    _domain.OnCleared                  += HandleCleared;

    _plantProgressionDomain.OnLevelUp += HandleUpgradeReady;

    foreach (var offering in _offeringAltars)
      offering.Initialize(this, iconProvider);
  }

  public bool OnOfferingPlaced(IItemDefinition item)
  {
    return _domain.PlaceItem(item);
  }

  public void OnOfferingRemoved(IItemDefinition item)
  {
    _domain.RemoveItem(item);
  }

  public void ConfirmCraft()
  {
    _domain.ConfirmCraft();
  }

  // =============================
  // Domain Handlers
  // =============================

  private void HandleUpgradeReady(IItemDefinition plant, bool buffed)
  {
    _plantProgressionDomain.OnPlantReady(plant, buffed);
  }

  private void HandleUpgradeReady(IItemDefinition plant, int newLevel)
  {
    StartCoroutine(OpenPopupAfterFill(plant));
  }

  private IEnumerator OpenPopupAfterFill(IItemDefinition plant)
  {
    bool filled = false;
    System.Action onFilled = () => filled = true;
    _progressionView.OnFilled += onFilled;
    yield return new WaitUntil(() => filled);
    _progressionView.OnFilled -= onFilled;

    var expData = _plantProgressionDomain.GetExp(plant);
    if (expData != null)
      _progressionView.SetProgressionImmediate(expData.Level, expData.CurrentExp, expData.MaxExp);

    _managerView.OnOpenUpgradePopup(plant.Name, plant.Key.Hash);
  }

  private void HandleSuggestionsChanged(System.Collections.Generic.List<AltarRecipeDefinition> recipes)
  {
    if (_domain.HasPendingCraft)
    {
      // slot[0] is occupied by the preview result icon — start suggestions from slot[1]
      _suggestionView?.ShowSuggestions(recipes, startSlot: 1);
    }
    else
    {
      _managerView.HideCraftPreview();
      _suggestionView?.ShowSuggestions(recipes);
    }
  }

  private void HandleCraftPreview(AltarRecipeDefinition recipe)
  {
    _managerView.ShowRecipePreview(recipe, ConfirmCraft);
  }

  private void HandleCraftReady(AltarRecipeDefinition recipe)
  {
    _managerView.HideCraftPreview();

    var instance = _itemFactory?.Create(recipe.ResultItemId);
    if (instance != null)
      _inventory?.AddItem(instance, 1);
  }

  private void HandleCleared()
  {
    _managerView.HideCraftPreview();
    ClearAllOfferings();
  }

  // =============================
  // Offerings
  // =============================

  private void ClearAllOfferings()
  {
    foreach (var offering in _offeringAltars)
      offering.Clear();
  }

  // =============================
  // Threshold
  // =============================

  private int GetRequiredCount()
  {
    if (_plantRequirementsPerLevel == null || _plantRequirementsPerLevel.Length == 0)
      return 1;

    int idx = Mathf.Clamp(_playerProgression.Level - 1, 0, _plantRequirementsPerLevel.Length - 1);
    return _plantRequirementsPerLevel[idx];
  }
}
