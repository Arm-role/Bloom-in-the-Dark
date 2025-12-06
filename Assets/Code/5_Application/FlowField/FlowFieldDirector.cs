using System;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldDirector : MonoBehaviour
{
    public static FlowFieldDirector Instance { get; private set; }

    // minimum interval per key (seconds)
    [Serializable] public struct KeyConfig { public string key; public float minInterval; }
    public KeyConfig[] keyConfigs;

    private Dictionary<string, float> _nextAllowed = new Dictionary<string, float>();
    private Dictionary<string, List<Vector3>> _pendingTargets = new Dictionary<string, List<Vector3>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var cfg in keyConfigs)
            _nextAllowed[cfg.key] = 0f;
    }
    // Called by FlowFieldRequestHub when a rebuild is requested
    // This collects targets and schedules an actual Build later when allowed.
    public void ScheduleBuild(string key, Vector3 target)
    {
        if (!_pendingTargets.TryGetValue(key, out var list))
        {
            list = new List<Vector3>();
            _pendingTargets[key] = list;
        }
        list.Add(target);
    }

    private void Update()
    {
        if (_pendingTargets.Count == 0) return;

        // iterate over a copy to allow modifications
        var keys = new List<string>(_pendingTargets.Keys);
        float now = Time.time;
        foreach (var k in keys)
        {
            float minInterval = GetMinIntervalForKey(k);
            if (!_nextAllowed.TryGetValue(k, out var t)) t = 0f;

            if (now < t) continue;

            // aggregate targets (we pass the distinct positions to the manager)
            var targets = _pendingTargets[k];
            if (targets == null || targets.Count == 0)
            {
                _pendingTargets.Remove(k);
                continue;
            }

            // Optionally: reduce duplicates / cluster target positions — here we just deduplicate by grid cell
            var uniq = DeduplicateTargets(targets);

            // build
            FlowFieldManager.Instance.BuildField(k, uniq);

            // clear pending and set cooldown
            _pendingTargets.Remove(k);
            _nextAllowed[k] = now + minInterval;
        }
    }

    private float GetMinIntervalForKey(string k)
    {
        foreach (var cfg in keyConfigs)
            if (cfg.key == k) return Mathf.Max(0.02f, cfg.minInterval);
        // default
        return 0.25f;
    }

    private List<Vector3> DeduplicateTargets(List<Vector3> source)
    {
        // Simple dedupe by world cell using world.GridConverter if available
        var result = new List<Vector3>();
        if (FlowFieldManager.Instance == null || FlowFieldManager.Instance.world == null) return source;

        var conv = FlowFieldManager.Instance.world.GridConverter;
        var seen = new HashSet<Vector3Int>();
        foreach (var v in source)
        {
            Vector3Int c = conv.WorldToCell(v);
            if (!seen.Contains(c))
            {
                seen.Add(c);
                result.Add(conv.CellToWorld(c)); // center
            }
        }
        return result;
    }
}
