using System;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private EItemType objectType;
    [SerializeField] private string objectName;

    public EItemType ObjectType => objectType;
    public string ObjectName => objectName;

    public Action OnLateUpdate { get; set; }
    public Action<InteractableObject> OnRequestDestruction { get; set; }

    private void LateUpdate() => OnLateUpdate?.Invoke();
    public void RequestDestruction() => OnRequestDestruction?.Invoke(this);
}