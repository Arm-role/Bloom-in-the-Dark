public class BarPresenter<T> where T : IResource
{
    private readonly T resource;
    private readonly IBarView view;

    public BarPresenter(T resource, IBarView view)
    {
        this.resource = resource;
        this.view = view;

        resource.OnChanged += OnChanged;
    }

    private void OnChanged(ResourceChangedEvent e)
    {
        switch (e.ChangeType)
        {
            case ResourceChangeType.Max:
            case ResourceChangeType.Fill:
                view.SetHealthImmediate(e.Current, e.Max);
                break;

            case ResourceChangeType.Value:
                view.SetHealth(e.Current, e.Max);
                break;
        }
    }
}