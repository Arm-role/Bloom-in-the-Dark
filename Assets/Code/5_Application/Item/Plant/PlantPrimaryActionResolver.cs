using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlantPrimaryActionResolver", menuName = "Game/Interaction/Resolvers/PlantPrimaryActionResolver")]
public class PlantPrimaryActionResolver : PrimaryActionTypeResolver
{
    public readonly Dictionary<string, IPrimaryAction> Actions = new Dictionary<string, IPrimaryAction>
    {
        {"GlowFlower", new GlowFlowerAction() },
        {"MelonBomber", new MelonBomberAction() }
    };
    public override IPrimaryAction Resolve(string itemName)
    {
        return Actions[itemName];
    }
}
