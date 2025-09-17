using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionService", menuName = "Game/Interaction/Interaction Service")]
public class InteractionService : ScriptableObject
{
    [SerializeField]
    private List<ResolverEntry<PrimaryActionTypeResolver>> _primaryActionResolvers;

    [SerializeField]
    private List<ResolverEntry<SecorndaryActionTypeResolver>> _secorndaryActionResolvers;

    [SerializeField]
    private List<ResolverEntry<DropTypeResolver>> _dropResolvers;

    [Serializable]
    public class ResolverEntry<T>
    {
        public EItemType Type;
        public T Resolver; 
    }

    private Dictionary<EItemType, PrimaryActionTypeResolver> _primaryResolverMap;
    private Dictionary<EItemType, SecorndaryActionTypeResolver> _secorndaryResolverMap;
    private Dictionary<EItemType, DropTypeResolver> _dropResolverMap;


    private void OnEnable()
    {
        _primaryResolverMap = new Dictionary<EItemType, PrimaryActionTypeResolver>();
        foreach (var entry in _primaryActionResolvers)
        {
            if (entry.Resolver != null && !_primaryResolverMap.ContainsKey(entry.Type))
            {
                _primaryResolverMap.Add(entry.Type, entry.Resolver);
            }
        }

        _secorndaryResolverMap = new Dictionary<EItemType, SecorndaryActionTypeResolver>();
        foreach (var entry in _secorndaryActionResolvers)
        {
            if (entry.Resolver != null && !_secorndaryResolverMap.ContainsKey(entry.Type))
            {
                _secorndaryResolverMap.Add(entry.Type, entry.Resolver);
            }
        }

        _dropResolverMap = new Dictionary<EItemType, DropTypeResolver>();
        foreach (var entry in _dropResolvers)
        {
            if (entry.Resolver != null && !_dropResolverMap.ContainsKey(entry.Type))
            {
                _dropResolverMap.Add(entry.Type, entry.Resolver);
            }
        }
    }

    public IPrimaryAction GetPrimaryActionResolve(EItemType objectType, string itemName)
    {
        if (_primaryResolverMap.TryGetValue(objectType, out PrimaryActionTypeResolver actionType))
        {
            return actionType.Resolve(itemName);
        }
        Debug.LogWarning("Not Found DropType");
        return null;
    }

    public ISecondaryAction GetSecondaryActionResolve(EItemType objectType, string itemName)
    {
        if (_secorndaryResolverMap.TryGetValue(objectType, out SecorndaryActionTypeResolver actionType))
        {
            return actionType.Resolve(itemName);
        }
        Debug.LogWarning("Not Found DropType");
        return null;
    }

    public IDrop GetDropResolve(EItemType itemType, Collider2D collider)
    {
        if (_dropResolverMap.TryGetValue(itemType, out DropTypeResolver dropType))
        {
            return dropType.Resolve(collider);
        }

        Debug.LogWarning("Not Found DropType");
        return null;
    }
}
