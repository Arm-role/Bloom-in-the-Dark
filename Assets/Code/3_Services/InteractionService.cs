using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionService", menuName = "Game/Interaction/Interaction Service")]
public class InteractionService : ScriptableObject
{
    [SerializeField]
    private List<ResolverEntry<ItemBehaviorTypeResolver>> _itemBehaviorResolvers;

    [Serializable]
    public class ResolverEntry<T>
    {
        public EItemType Type;
        public T Resolver; 
    }

    private Dictionary<EItemType, ItemBehaviorTypeResolver> _itemBehaviorResolverMap;

    private void OnEnable()
    {
        _itemBehaviorResolverMap = new Dictionary<EItemType, ItemBehaviorTypeResolver>();
        foreach (var entry in _itemBehaviorResolvers)
        {
            if (entry.Resolver != null && !_itemBehaviorResolverMap.ContainsKey(entry.Type))
            {
                _itemBehaviorResolverMap.Add(entry.Type, entry.Resolver);
            }
        }
    }

    public IItemBehavior GetItemBehaviorResolve(EItemType objectType, string itemName, Collider2D collider = null)
    {
        if (_itemBehaviorResolverMap.TryGetValue(objectType, out ItemBehaviorTypeResolver actionType))
        {
            if(collider != null)
            {
                return actionType.Resolve(itemName, collider.tag);
            }
            return actionType.Resolve(itemName);
        }
        Debug.LogWarning("Not Found PrimaryActionType");
        return null;
    }
}
