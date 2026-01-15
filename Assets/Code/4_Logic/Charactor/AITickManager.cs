using System.Collections.Generic;
using System;
using UnityEngine;

public class AITickManager : MonoBehaviour
{
    public static AITickManager Instance { get; private set; }
    private struct TickEntry { public int id; public Action cb; public float interval; public float nextTime; }
    private readonly List<TickEntry> _entries = new List<TickEntry>();
    private int _nextId = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        Instance = this;
    }

    private void Update()
    {
        float t = Time.time;
        for (int i = 0; i < _entries.Count; i++)
        {
            var e = _entries[i];
            if (t >= e.nextTime)
            {
                try { e.cb?.Invoke(); } catch (Exception ex) { Debug.LogException(ex); }
                e.nextTime = t + e.interval;
                _entries[i] = e;
            }
        }
    }

    public int Register(Action cb, float hz)
    {
        if (hz <= 0f) hz = 1f;
        var entry = new TickEntry { id = _nextId++, cb = cb, interval = 1f / hz, nextTime = Time.time + (1f / hz) };
        _entries.Add(entry);
        return entry.id;
    }

    public void Unregister(int id) { _entries.RemoveAll(e => e.id == id); }
}
