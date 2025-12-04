using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class NavigationSystem : MonoBehaviour
{
    public static NavigationSystem Instance { get; private set; }
    public NavigationGraph Graph { get; private set; }
    public Pathfinder Pathfinder { get; private set; }

    [Header("Config")]
    public bool autoBuildOnStart = false;
    public WorldTileManager world;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        Instance = this;
    }

    private void Start()
    {
        if (autoBuildOnStart && world != null) BuildFromWorld(world);
    }

    public void BuildFromWorld(WorldTileManager worldTileManager)
    {
        if (worldTileManager == null) { Debug.LogError("NavigationSystem: null world"); return; }
        var g = new NavigationGraph();
        foreach (var t in worldTileManager.GetTileBaseDataStates())
        {
            if (t == null) continue;
            if (t.HasObstacle) continue; // skip blocked tiles
            g.AddNode(t.CellPos, t.WorldCenter);
        }
        g.BuildAdjacency();
        Graph = g;
        Pathfinder = new Pathfinder(Graph);
        Debug.Log($"NavigationSystem: built graph nodes={Graph.NodeCount}");
    }

    public List<Vector3> FindPathWorld(Vector3Int startTile, Vector3Int goalTile)
    {
        if (Pathfinder == null) return null;
        return Pathfinder.FindPath(startTile, goalTile);
    }

    public Vector3Int ToTile(Vector3 worldPosition)
    {
        if (world != null)
        {
            var mi = world.GetType().GetMethod("ToTilePos", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (mi != null)
            {
                object res = mi.Invoke(world, new object[] { worldPosition });
                if (res is Vector3Int vi) return vi;
            }
            var prop = world.GetType().GetProperty("GridConverter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null)
            {
                var conv = prop.GetValue(world);
                if (conv != null)
                {
                    var mi2 = conv.GetType().GetMethod("WorldToCell", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                              ?? conv.GetType().GetMethod("WorldToCellPosition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (mi2 != null)
                    {
                        object r = mi2.Invoke(conv, new object[] { worldPosition });
                        if (r is Vector3Int vi2) return vi2;
                    }
                }
            }
        }
        return new Vector3Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y), 0);
    }
}