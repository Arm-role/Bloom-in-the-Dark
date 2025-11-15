using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MockSettings", menuName = "Game/Mock Settings")]
public class MockSettings : ScriptableObject
{
    [Header("ITEM INVENTORY")]
    public List<Item> items;
}