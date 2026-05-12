using UnityEngine;

public class OfferingAltarController : MonoBehaviour
{
  private AltarController _altar;
  private IItemInstance _heldInstance;

  public bool IsOccupied => _heldInstance != null;

  public void Initialize(AltarController altar)
  {
    _altar = altar;
  }

  public bool TryPlaceItem(IItemInstance instance)
  {
    if (IsOccupied) return false;

    Debug.Log($"Trying to place {instance.Data.Name} on altar");

    if (!_altar.OnOfferingPlaced(instance.Data)) return false;
    _heldInstance = instance;
    return true;
  }

  public IItemInstance RemoveItem()
  {
    if (!IsOccupied) return null;

    Debug.Log($"Removing {_heldInstance.Data.Name} from altar");

    var instance = _heldInstance;
    _heldInstance = null;
    _altar.OnOfferingRemoved(instance.Data);
    return instance;
  }

  public void Clear()
  {
    _heldInstance = null;
  }
}
