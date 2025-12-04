using System.Collections.Generic;
using System;
using UnityEngine;

public class AITickManager : MonoBehaviour
{
    public static AITickManager Instance { get; private set; }

    private class TickEntry
    {
        public Action Callback;
        public float Interval;
        public float NextTime;
        public bool Active;
    }

    private readonly List<TickEntry> _entries = new List<TickEntry>();
    private readonly Stack<int> _freeIds = new Stack<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        float t = Time.time;

        for (int i = 0; i < _entries.Count; i++)
        {
            TickEntry e = _entries[i];
            if (e == null || !e.Active) continue;

            if (t >= e.NextTime)
            {
                try { e.Callback?.Invoke(); }
                catch (Exception ex) { Debug.LogException(ex); }

                e.NextTime = t + e.Interval;
            }
        }
    }

    public int Register(Action callback, float hz)
    {
        if (callback == null) return -1;
        if (hz <= 0f) hz = 1f;

        TickEntry entry = new TickEntry()
        {
            Callback = callback,
            Interval = 1f / hz,
            NextTime = Time.time + (1f / hz),
            Active = true
        };

        if (_freeIds.Count > 0)
        {
            int reused = _freeIds.Pop();
            _entries[reused] = entry;
            return reused;
        }
        else
        {
            _entries.Add(entry);
            return _entries.Count - 1;
        }
    }

    public void Unregister(int id)
    {
        if (id < 0 || id >= _entries.Count) return;

        _entries[id] = null;
        _freeIds.Push(id);
    }
}
