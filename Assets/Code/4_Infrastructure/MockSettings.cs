using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MockSettings", menuName = "Game/Mock Settings")]
public class MockSettings : ScriptableObject
{
  [Header("EMPTY ITEM")]
  public ItemDefinition EmptyItem;

  [Header("ITEM INVENTORY")]
  public List<ItemDefinition> Items;
}