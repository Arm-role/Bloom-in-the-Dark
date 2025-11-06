using UnityEngine;

[CreateAssetMenu(fileName = "ResourceItemBehaviorResolver", menuName = "Game/Interaction/Resolvers/ResourceItemBehaviorResolver")]
public class ResourceItemBehaviorResolver : ItemBehaviorTypeResolver
{
    public override IItemBehavior Resolve(string itemName, InteractionTarget target)
    {
        if (target.IsTile && target.TileData is BaseTileData)
        {
            return new ActiveTargetAction();
        }

        Debug.Log($"[SeedResolver] {itemName} can only be planted on soil tile.");
        return null;
    }
}