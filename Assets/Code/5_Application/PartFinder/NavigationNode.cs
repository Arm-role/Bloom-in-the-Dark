using System.Collections.Generic;
using UnityEngine;

public class NavigationNode
{
    public Vector3Int TilePos;
    public Vector3 WorldCenter;
    public List<int> Neighbors = new List<int>();

    public NavigationNode(Vector3Int tilePos, Vector3 worldCenter)
    {
        TilePos = tilePos;
        WorldCenter = worldCenter;
    }
}
