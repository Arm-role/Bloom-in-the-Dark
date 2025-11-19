using System.Collections.Generic;

public static class TileLayerPriority
{
    private static readonly Dictionary<ETileLayerType, int> Priority = new()
    {
        { ETileLayerType.Crop, 4 },
        { ETileLayerType.Interactable, 3 },
        { ETileLayerType.Object, 2 },
        { ETileLayerType.Building, 1 },
        { ETileLayerType.Ground, 0 },
    };

    public static int GetPriority(ETileLayerType layer)
        => Priority.TryGetValue(layer, out var p) ? p : 0;
}