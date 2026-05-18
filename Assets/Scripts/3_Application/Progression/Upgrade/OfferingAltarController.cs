using System.Collections;
using UnityEngine;

public class OfferingAltarController : MonoBehaviour
{
  private AltarController _altar;
  private IItemIconProvider _iconProvider;
  private IItemInstance _heldInstance;
  private IOfferingAltarPreview _preview;

  [SerializeField] private float _interactCooldown = 0.5f;
  [SerializeField] private float _placementDelay = 1.0f;
  [SerializeField] private SpriteRenderer _altarRenderer;
  [SerializeField] private Sprite _normalSprite;
  [SerializeField] private Sprite _brokenSprite;

  private float _lastInteractTime = -Mathf.Infinity;
  private Coroutine _placementCoroutine;
  private bool _domainCommitted;
  private bool _isLocked;

  private bool IsOnCooldown => Time.time - _lastInteractTime < _interactCooldown;

  public bool IsOccupied => _heldInstance != null;

  public void Lock()
  {
    _isLocked = true;
    if (_altarRenderer != null) _altarRenderer.sprite = _brokenSprite;
  }

  public void Unlock()
  {
    _isLocked = false;
    if (_altarRenderer != null) _altarRenderer.sprite = _normalSprite;
  }

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
    if (_isLocked) return false;
    if (IsOnCooldown) return false;
    if (IsOccupied) return false;

    _heldInstance = instance;
    _domainCommitted = false;
    _lastInteractTime = Time.time;
    _preview.Show(_iconProvider.GetIcon(instance.Data.ID));
    _placementCoroutine = StartCoroutine(CommitAfterDelay(instance));
    return true;
  }

  private IEnumerator CommitAfterDelay(IItemInstance instance)
  {
    yield return new WaitForSeconds(_placementDelay);

    _placementCoroutine = null;

    if (_heldInstance != instance)
      yield break;

    if (!_altar.OnOfferingPlaced(instance.Data))
    {
      _heldInstance = null;
      _preview.Hide();
      yield break;
    }

    _domainCommitted = true;

    // Domain may have cleared synchronously (e.g. instant upgrade).
    if (!IsOccupied)
      _preview.Hide();
  }

  public IItemInstance RemoveItem()
  {
    if (IsOnCooldown) return null;
    if (!IsOccupied) return null;

    var instance = _heldInstance;
    _heldInstance = null;
    _lastInteractTime = Time.time;

    if (_placementCoroutine != null)
    {
      // Item removed before the delay expired — domain was never notified, no need to inform it.
      StopCoroutine(_placementCoroutine);
      _placementCoroutine = null;
    }
    else if (_domainCommitted)
    {
      _altar.OnOfferingRemoved(instance.Data);
    }

    _domainCommitted = false;
    _preview.Hide();
    return instance;
  }

  public void Clear()
  {
    if (_placementCoroutine != null)
    {
      StopCoroutine(_placementCoroutine);
      _placementCoroutine = null;
    }
    _domainCommitted = false;
    _heldInstance = null;
    _preview?.Hide();
  }
}
