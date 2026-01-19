using System.Collections;
using UnityEngine;

public interface IItemInstance
{
    IItemData Data { get; }
    float GetStat(EItemStatType stat);
    bool HasStat(EItemStatType stat);
    
    T GetProperty<T>(EItemProperty property);
    bool HasProperty(EItemProperty property);
}