using System;

public class BarPresenter<T> : IDisposable where T : IResource
{
  private readonly T _resource;
  private readonly IBarView _view;

  public BarPresenter(T resource, IBarView view)
  {
    _resource = resource;
    _view = view;

    _view.SetHealthImmediate(_resource.Current, _resource.Max);
    _resource.OnChanged += OnChanged;
  }

  public void Dispose()
  {
    _resource.OnChanged -= OnChanged;
  }

  private void OnChanged(ResourceChangedEvent e)
  {
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
