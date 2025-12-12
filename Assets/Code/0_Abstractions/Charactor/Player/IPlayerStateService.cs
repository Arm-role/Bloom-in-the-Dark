using UnityEngine;

public interface IPlayerStateService
{
    IItemInstance GetCurrentSelectedItem();

    string GetCurrentHeldTileID();
}