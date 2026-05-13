using UnityEngine;

public struct PlacementShape
{
    public Vector2Int Size;
    public bool[,] Mask;

    public PlacementShape(Vector2Int size)
    {
        Size = size;
        Mask = new bool[size.x, size.y];
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                Mask[x, y] = true;
    }

    public static PlacementShape Full(Vector2Int size) => new PlacementShape(size);

    public bool IsCellUsed(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Size.x || y >= Size.y) return false;
        return Mask[x, y];
    }
}