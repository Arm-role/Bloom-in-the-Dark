using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AltarController : MonoBehaviour
{
  [SerializeField] private GameTagAsset _moonBloomTag;
  [SerializeField] private GameTagAsset _plantTag;
  [SerializeField] private int[] _plantRequirementsPerLevel = { 1 };
  [SerializeField] private OfferingAltarController[] _offeringAltars;
  [SerializeField] private Vector2 _npcExitOffset = Vector2.up;

  [Header("Phase")]
  [SerializeField] private int[] _unlockedSlotsPerPhase = { 2, 4, 6 };
  [SerializeField] private GameObject[] _altarVisualPerPhase;

  private AltarDomain _domain;
  private IUpgradeManagerView _managerView;
  private IProgressionView _progressionView;
  private IAltarRecipeSuggestionView _suggestionView;
  private PlantProgressionDomain _plantProgressionDomain;
  private PlayerProgression _playerProgression;
  private ItemFactory _itemFactory;
  private PlayerInventory _inventory;
  private IEntitySpawner _entitySpawner;
  private GlobalUpgradeDomain _globalUpgrade;
  private int _phaseLevel;

  public event Action<NpcController> OnNpcCrafted;

  public void Initialize(
    IItemIconProvider iconProvider,
    AltarRecipeDatabase recipeDatabase,
    IUpgradeManagerView managerView,
    IProgressionView progressionView,
    IAltarRecipeSuggestionView suggestionView,
    PlayerProgression playerProgression,
    ItemFactory itemFactory,
    PlayerInventory inventory,
    IEntitySpawner entitySpawner = null,
    GlobalUpgradeDomain globalUpgrade = null)
  {
    _managerView = managerView;
    _progressionView = progressionView;
    _suggestionView = suggestionView;
    _playerProgression = playerProgression;
    _itemFactory = itemFactory;
    _inventory = inventory;
    _entitySpawner = entitySpawner;
    _globalUpgrade = globalUpgrade;

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

    if (_globalUpgrade != null)
      _globalUpgrade.OnAltarPhaseAdvance += OnAltarPhaseAdvance;

    ApplyPhase(0);

    foreach (var offering in _offeringAltars)
      offering.Initialize(this, iconProvider);
  }

  private void OnDisable()
  {
    if (_globalUpgrade != null)
      _globalUpgrade.OnAltarPhaseAdvance -= OnAltarPhaseAdvance;
  }

  private void OnAltarPhaseAdvance()
  {
    _phaseLevel = Mathf.Min(_phaseLevel + 1, _unlockedSlotsPerPhase.Length - 1);
    ApplyPhase(_phaseLevel);
  }

  private void ApplyPhase(int phase)
  {
    int unlocked = _unlockedSlotsPerPhase[Mathf.Clamp(phase, 0, _unlockedSlotsPerPhase.Length - 1)];
    for (int i = 0; i < _offeringAltars.Length; i++)
    {
      if (i < unlocked)
        _offeringAltars[i].Unlock();
      else
        _offeringAltars[i].Lock();
    }

    if (_altarVisualPerPhase != null)
    {
      for (int i = 0; i < _altarVisualPerPhase.Length; i++)
        _altarVisualPerPhase[i]?.SetActive(i == phase);
    }
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

    _managerView.OnOpenUpgradePopup(plant.DisplayName, plant.Key.Hash);
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

    if (recipe.IsNpcCraft)
      _ = SpawnCraftedNpcAsync(recipe);
    else
      CraftItem(recipe);
  }

  private void CraftItem(AltarRecipeDefinition recipe)
  {
    var instance = _itemFactory?.Create(recipe.ResultItemId);
    if (instance != null)
      _inventory?.AddItem(instance, 1);
  }

  private async Task SpawnCraftedNpcAsync(AltarRecipeDefinition recipe)
  {
    if (_entitySpawner == null)
      throw new System.InvalidOperationException($"{nameof(IEntitySpawner)} was not injected into {nameof(AltarController)}");

    var spawnPos = transform.position + (Vector3)_npcExitOffset;
    var npc = await _entitySpawner.SpawnNpc(recipe.ResultNpcId, spawnPos);
    if (npc == null) return;

    var destination = transform.position + (Vector3)recipe.NpcSpawnOffset;
    npc.WalkToThenPatrol(destination);
    OnNpcCrafted?.Invoke(npc);
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
