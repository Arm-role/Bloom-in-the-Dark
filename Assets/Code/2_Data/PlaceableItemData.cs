using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placeable Item", menuName = "Game/Placeable Item")]
public class PlaceableItemData : ScriptableObject
{
    [Tooltip("The actual prefab that will be instantiated in the world.")]
    public GameObject Prefab;

    [Tooltip("The sprite used for the placement preview.")]
    public Sprite PreviewSprite;

    // public bool CanBePlacedOnWater;
    // public Vector2Int Size = Vector2Int.one;
}
