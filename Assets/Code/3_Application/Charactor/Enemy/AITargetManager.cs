using System;
using System.Collections.Generic;
using UnityEngine;

public class AITargetManager : MonoBehaviour
{
    public static AITargetManager Instance { get; private set; }

    public Transform Player;

    public List<Transform> Bases = new List<Transform>();

    public event Action OnTargetListChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterBase(Transform b)
    {
        if (!Bases.Contains(b))
        {
            Bases.Add(b);
            OnTargetListChanged?.Invoke();
        }
    }

    public void RemoveBase(Transform b)
    {
        if (Bases.Contains(b))
        {
            Bases.Remove(b);
            OnTargetListChanged?.Invoke();
        }
    }

    public Transform GetNearestBase(Vector3 pos)
    {
        float best = Mathf.Infinity;
        Transform result = null;

        foreach (var b in Bases)
        {
            if (b == null) continue;
            float d = Vector2.Distance(pos, b.position);
            if (d < best)
            {
                best = d;
                result = b;
            }
        }

        return result;
    }
}
