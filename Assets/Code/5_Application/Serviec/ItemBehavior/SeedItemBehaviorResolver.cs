using UnityEngine;

[CreateAssetMenu(fileName = "SeedItemBehaviorResolver", menuName = "Game/Interaction/Resolvers/SeedItemBehaviorResolver")]
public class SeedItemBehaviorResolver : ItemBehaviorTypeResolver
{
    public override IItemBehavior Resolve(string itemName, InteractionTarget target)
    {
        return new ActiveTargetAction();


        //Debug.Log($"[SeedResolver] {itemName} can only be planted on soil tile.");
        //return null;
    }
}
