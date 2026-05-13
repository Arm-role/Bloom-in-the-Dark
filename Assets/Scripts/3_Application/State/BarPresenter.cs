public class BarPresenter<T> where T : IResource
{
  private readonly T _resource;
  private readonly IBarView _view;

  public BarPresenter(T resource, IBarView view)
  {
    _resource = resource;
    _view = view;

    view.SetHealthImmediate(_resource.Current, _resource.Max);
    
    //Debug.Log($"resource Register");
    resource.OnChanged += OnChanged;
  }

  private void OnChanged(ResourceChangedEvent e)
  {
    //Debug.Log($"resource changed {e.Current}, {e.Max}");
    switch (e.ChangeType)
    {
      case ResourceChangeType.Max:
      case ResourceChangeType.Fill:
        _view.SetHealthImmediate(e.Current, e.Max);
        break;

      case ResourceChangeType.Value:
        _view.SetHealth(e.Current, e.Max);
        break;
    }
  }
}