using System.Collections.Generic;
using UnityEngine;

public interface IItemData
{
    int ID { get; }
    string Name { get; }
    EItemType Type { get; }
    IItemInteractionProfile InteractionProfile { get; }
    
    Sprite Icon { get; }
    int MaxStackSize { get; }
    
    bool SupportsStat(EItemStatType stat);
    float GetBaseStat(EItemStatType stat);
    float GetPerLevelStat(EItemStatType stat);
    
    bool SupportsProperty(EItemProperty property);
    T GetProperty<T>(EItemProperty property);
}