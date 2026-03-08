using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MockSettings", menuName = "Game/Mock Settings")]
public class MockSettings : ScriptableObject
{
  [Header("EMPTY ITEM")]
  public EmptyItem EmptyItem;

  [Header("ITEM INVENTORY")]
  public List<Item> Items;
}