using System;

public interface IResource
{
    float Current { get; }
    float Max { get; }

    event Action<ResourceChangedEvent> OnChanged;
}