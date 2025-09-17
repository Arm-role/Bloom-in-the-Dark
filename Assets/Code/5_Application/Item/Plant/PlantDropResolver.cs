using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantDropResolver", menuName = "Game/Interaction/Resolvers/PlantDropResolver")]
public class PlantDropResolver : DropTypeResolver
{
    public readonly Dictionary<string, IDrop> Actions = new Dictionary<string, IDrop> { };

    public override IDrop Resolve(Collider2D itemName)
    {
        return Actions[itemName.tag];
    }
}