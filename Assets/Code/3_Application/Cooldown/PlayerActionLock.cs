
public sealed class PlayerActionLock
{
  private readonly ITimeSource _time;

  private bool _isBusy;
  private float _unlockTime;
  private string _currentAction;

  public bool IsBusy => _isBusy;
  public string CurrentAction => _currentAction;

  public PlayerActionLock(ITimeSource time)
  {
    _time = time;
  }

  public bool TryLock(string actionKey, float duration)
  {
    Update();

    if (_isBusy)
      return false;

    _isBusy = true;
    _currentAction = actionKey;
    _unlockTime = _time.Now + duration;

    return true;
  }

  public void Update()
  {
    if (!_isBusy)
      return;

    if (_time.Now >= _unlockTime)
    {
      _isBusy = false;
      _currentAction = null;
    }
  }

  public void ForceRelease()
  {
    _isBusy = false;
    _currentAction = null;
  }
}
