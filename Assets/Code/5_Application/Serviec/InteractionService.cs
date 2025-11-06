using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionService", menuName = "Game/Interaction/Interaction Service")]
public class InteractionService : ScriptableObject
{
    [SerializeField]
    private List<ResolverEntry> _itemBehaviorResolvers;

    [Serializable]
    public class ResolverEntry
    {
        public EItemType Type;
        public ItemBehaviorTypeResolver Resolver;
    }

    private Dictionary<EItemType, ItemBehaviorTypeResolver> _resolverMap;

    private void OnEnable()
    {
        _resolverMap = new Dictionary<EItemType, ItemBehaviorTypeResolver>();

        foreach (var entry in _itemBehaviorResolvers)
        {
            if (entry.Resolver == null) continue;
            if (_resolverMap.ContainsKey(entry.Type)) continue;

            _resolverMap.Add(entry.Type, entry.Resolver);
        }
    }

    public IItemBehavior GetItemBehaviorResolve(EItemType itemType, string itemName, InteractionTarget target = default)
    {
        if (!_resolverMap.TryGetValue(itemType, out var resolver))
        {
            Debug.LogWarning($"❌ [InteractionService] No resolver found for item type {itemType}");
            return null;
        }

        return resolver.Resolve(itemName, target);
    }
}
