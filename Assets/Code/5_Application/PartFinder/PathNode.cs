using UnityEngine;

public class PathNode
{
    public Vector3Int Pos;
    public float G;
    public float H;
    public PathNode Parent;

    public float F => G + H;

    public PathNode(Vector3Int pos, float g, float h, PathNode parent)
    {
        Pos = pos;
        G = g;
        H = h;
        Parent = parent;
    }
}
