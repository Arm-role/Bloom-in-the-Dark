using UnityEngine;

public class AltarController : MonoBehaviour
{
  [SerializeField] private GameTagAsset _moonBloomTag;
  [SerializeField] private GameTagAsset _plantTag;

  private AltarDomain _domain;
  private IUpgradeRequestView _requestView;

  public void Initialize(
    RequestDatabase requestDatabase,
    IUpgradeRequestView requestView,
    IUpgradeManagerView managerView)
  {
    _requestView = requestView;

    _domain = new AltarDomain(
      requestDatabase,
      _moonBloomTag.RuntimeTag,
      _plantTag.RuntimeTag);

    _domain.OnUpgradeReady += (plant, buffed) =>
      managerView.OnOpenUpgradePopup(plant.Name, plant.Key.Hash);

    _domain.OnCraftReady += request =>
      managerView.OnOpenUpgradePopup(request.UpgradeName, request.GameKeyId);

    _domain.OnCraftProgressChanged += bars =>
    {
      // TODO: map to RequestBarViewModel and show progress
    };

    _domain.OnCleared += () => _requestView.Hide();
  }

  public void PlaceItem(IItemDefinition item)
  {
    _domain.PlaceItem(item);
  }
}
