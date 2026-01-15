using System.Collections.Generic;

public class PlantItemInstance : ItemInstanceBase
{
    public ICooldownOwner CooldownOwner { get; }

    public PlantItemInstance(IItemData data)
        : base(data)
    {
        CooldownOwner = new ItemCooldownComponent();
    }
}