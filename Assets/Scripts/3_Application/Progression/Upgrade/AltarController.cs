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
  private PlantProgressionDomain _plantProgressionDomain;
  private PlayerProgression _playerProgression;

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
    _playerProgression = playerProgression;

    _plantProgressionDomain = new PlantProgressionDomain(progressionView);

    _domain = new AltarDomain(
      requestDatabase,
      _moonBloomTag.RuntimeTag,
      _plantTag.RuntimeTag,
      GetRequiredCount);

    _domain.OnUpgradeProgressChanged += HandleUpgradeProgress;
    _domain.OnUpgradeReady += HandleUpgradeReady;
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
    _plantProgressionDomain.OnPlantReady(plant, buffed);
  }

  private void HandleUpgradeReady(IItemDefinition plant, int newLevel)
  {
    _managerView.OnOpenUpgradePopup(plant.Name, plant.Key.Hash);
  }

  private void HandleCraftPreview(UpgradeRequestDefinition request)
  {
    _managerView.ShowCraftPreview(request, ConfirmCraft);
  }

  private void HandleCraftReady(UpgradeRequestDefinition request)
  {
    _managerView.HideCraftPreview();
    _managerView.OnOpenUpgradePopup(request.UpgradeName, request.GameKeyId);
  }

  private void HandleCleared()
  {
    _requestView.Hide();
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
