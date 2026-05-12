using UnityEngine;

public class OfferingAltarController : MonoBehaviour
{
  private AltarController _altar;
  private IItemIconProvider _iconProvider;
  private IItemInstance _heldInstance;
  private IOfferingAltarPreview _preview;

  [SerializeField] private float _interactCooldown = 0.5f;
  private float _lastInteractTime = -Mathf.Infinity;

  private bool IsOnCooldown => Time.time - _lastInteractTime < _interactCooldown;

  public bool IsOccupied => _heldInstance != null;

  private void Start()
  {
    _preview = GetComponentInChildren<IOfferingAltarPreview>();
  }

  public void Initialize(AltarController altar, IItemIconProvider iconProvider)
  {
    _altar = altar;
    _iconProvider = iconProvider;
  }

  public bool TryPlaceItem(IItemInstance instance)
  {
    if (IsOnCooldown) return false;
    if (IsOccupied) return false;
    if (!_altar.OnOfferingPlaced(instance.Data)) return false;

    _heldInstance = instance;
    _lastInteractTime = Time.time;
    _preview.Show(_iconProvider.GetIcon(instance.Data.ID));
    return true;
  }

  public IItemInstance RemoveItem()
  {
    if (IsOnCooldown) return null;
    if (!IsOccupied) return null;

    var instance = _heldInstance;
    _heldInstance = null;
    _lastInteractTime = Time.time;
    _altar.OnOfferingRemoved(instance.Data);
    _preview.Hide();
    return instance;
  }

  public void Clear()
  {
    _heldInstance = null;
  }
}