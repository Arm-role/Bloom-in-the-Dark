using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarController : MonoBehaviour
{
  [SerializeField] private GameTagAsset _moonBloomTag;
  [SerializeField] private GameTagAsset _plantTag;
  [SerializeField] private int[] _plantRequirementsPerLevel = { 1 };
  [SerializeField] private OfferingAltarController[] _offeringAltars;

  private AltarDomain _domain;
  private IUpgradeRequestView _requestView;
  private IUpgradeManagerView _managerView;
  private IProgressionView _progressionView;
  private PlantProgressionDomain _plantProgressionDomain;
  private PlayerProgression _playerProgression;

  // True while HandleCleared is executing as a consequence of FireUpgrade,
  // so the request view (which just showed progress) is not immediately hidden.
  private bool _pendingUpgradeDisplay;

  public void Initialize(
    IItemIconProvider iconProvider,
    RequestDatabase requestDatabase,
    IUpgradeRequestView requestView,
    IUpgradeManagerView managerView,
    IProgressionView progressionView,
    PlayerProgression playerProgression)
  {
    _requestView = requestView;
    _managerView = managerView;
    _progressionView = progressionView;
    _playerProgression = playerProgression;

    _plantProgressionDomain = new PlantProgressionDomain(progressionView);

    _domain = new AltarDomain(
      requestDatabase,
      _moonBloomTag.RuntimeTag,
      _plantTag.RuntimeTag,
      GetRequiredCount);

    _domain.OnUpgradeProgressChanged += HandleUpgradeProgress;
    _domain.OnUpgradeReady += HandleUpgradeReady;
    _domain.OnCraftProgressChanged += HandleCraftProgress;
    _domain.OnCraftPreviewReady += HandleCraftPreview;
    _domain.OnCraftReady += HandleCraftReady;
    _domain.OnCleared += HandleCleared;

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

  private void HandleUpgradeProgress(int current, int required, IItemDefinition plant)
  {
    _requestView.SetSlots(new List<RequestBarViewModel>
    {
      new RequestBarViewModel
      {
        upgradeName = plant.Name,
        slotViewModels = new List<RequestSlotViewModel>
        {
          new RequestSlotViewModel
          {
            ItemId = plant.ID,
            Amount = required,
            CurrentAmount = current
          }
        }
      }
    });
  }

  private void HandleUpgradeReady(IItemDefinition plant, bool buffed)
  {
    _pendingUpgradeDisplay = true;
    _plantProgressionDomain.OnPlantReady(plant, buffed);
  }

  private void HandleUpgradeReady(IItemDefinition plant, int newLevel)
  {
    _requestView.Hide();
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

  private void HandleCraftProgress(List<UpgradeRequestDefinition> matching)
  {
    if (matching == null || matching.Count == 0)
    {
      _requestView.Hide();
      return;
    }

    var request = matching[0];
    var snapshot = _domain.GetCraftSnapshot();
    var slots = new List<RequestSlotViewModel>();

    foreach (var ingredient in request.Ingredients)
    {
      snapshot.TryGetValue(ingredient.item.RuntimeTag.Hash, out int current);
      slots.Add(new RequestSlotViewModel
      {
        ItemId = ingredient.item.RuntimeTag.Hash,
        Amount = ingredient.amount,
        CurrentAmount = current
      });
    }

    _requestView.SetSlots(new List<RequestBarViewModel>
    {
      new RequestBarViewModel { upgradeName = request.UpgradeName, slotViewModels = slots }
    });
  }

  private void HandleCraftPreview(UpgradeRequestDefinition request)
  {
    _requestView.Hide();
    _managerView.ShowCraftPreview(request, ConfirmCraft);
  }

  private void HandleCraftReady(UpgradeRequestDefinition request)
  {
    _managerView.HideCraftPreview();
    _managerView.OnOpenUpgradePopup(request.UpgradeName, request.GameKeyId);
  }

  private void HandleCleared()
  {
    // When clearing is a consequence of FireUpgrade, suppress the Hide so the
    // progress bar the player just saw isn't killed in the same frame it appeared.
    // For level-up upgrades the bar is hidden by HandleUpgradeReady(int) just before
    // the popup opens; for non-level-up rounds it stays briefly visible until the
    // next placement overwrites it via SetSlots.
    if (!_pendingUpgradeDisplay)
      _requestView.Hide();

    _pendingUpgradeDisplay = false;
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
