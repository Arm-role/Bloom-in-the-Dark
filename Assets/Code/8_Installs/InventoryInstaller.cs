public class InventoryInstaller
{
    public void Install(DIContainerBase container, GameSceneInstaller scene)
    {
        var inventoryState = container.Get<InventoryState>();

        inventoryState.Initialize(
            scene.UIManager.OnEnterInventory,
            scene.UIManager.OnExitInventory
            );

        var hotbarState = new HotbarState(scene.GameSetting.HotbarSize);

        var inventory = new PlayerInventory(
            hotbarState,
            scene.GameSetting.HotbarSize,
            scene.GameSetting.InventorySize
        );

        foreach (var item in scene.MockSettings.items)
        {
            switch (item.Type)
            {
                case EItemType.Tool: inventory.AddItem(ItemFactory.Create(item), 1); break;
                case EItemType.Plant: inventory.AddItem(ItemFactory.Create(item), 10); break;
                case EItemType.Seed: inventory.AddItem(ItemFactory.Create(item), 10); break;
                case EItemType.Building: inventory.AddItem(ItemFactory.Create(item), 10); break;
            }
        }

        container.Register(hotbarState);
        container.Register(inventory);

        scene.HotbarController.Initialize(scene.InputRender, hotbarState);

        scene.InventoryController.Initialize(
            inventory,
            hotbarState,
            scene.HotbarInventoryView,
            scene.MainInventoryView,
            scene.DragGhost
        );
    }
}
