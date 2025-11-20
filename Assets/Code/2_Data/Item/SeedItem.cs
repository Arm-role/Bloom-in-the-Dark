using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new SeedItem", menuName = "Item/New SeedItem")]
public class SeedItem : Item, IEnergyReduce
{
    [Header("SeedData")]
    [SerializeField] private string plantName;
    [SerializeField] private int energyReduceEachAction;

    public string PlantName => plantName;
    public float EnergyReduceEachAction => energyReduceEachAction;

    public override EItemType Type => EItemType.Seed;
    public override EItemStategyType StategyType => EItemStategyType.DirectInteract;
    public override int MaxStackSize => 64;
}