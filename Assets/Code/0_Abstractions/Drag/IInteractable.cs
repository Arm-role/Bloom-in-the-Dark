using UnityEngine;

public interface IInteractable
{
    GameObject gameObject { get; }
    EItemType ObjectType { get; }
    string ObjectName { get; }

    void RequestDestruction();
}
