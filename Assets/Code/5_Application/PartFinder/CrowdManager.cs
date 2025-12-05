using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-60)]
public class CrowdManager : MonoBehaviour
{
    public static CrowdManager Instance;

    private static readonly List<EnemyCrowdAgent> agents = new List<EnemyCrowdAgent>();
    private const float avoidanceWeight = 1.8f;
    private const float neighborRadius = 2.5f;

    void Awake()
    {
        Instance = this;
    }

    public static void Register(EnemyCrowdAgent a)
    {
        if (!agents.Contains(a))
            agents.Add(a);
    }

    public static void Unregister(EnemyCrowdAgent a)
    {
        agents.Remove(a);
    }

    void FixedUpdate()
    {
        int count = agents.Count;
        for (int i = 0; i < count; i++)
        {
            var a = agents[i];
            if (a == null) continue;

            Vector2 avoidance = Vector2.zero;

            // local neighbors
            for (int j = 0; j < count; j++)
            {
                if (i == j) continue;

                var b = agents[j];
                if (b == null) continue;

                Vector2 diff = (Vector2)(a.transform.position - b.transform.position);
                float dist = diff.magnitude;
                float combinedRadius = a.Radius + b.Radius;

                if (dist < neighborRadius && dist > 0.001f)
                {
                    float penetration = combinedRadius - dist;

                    if (penetration > 0f)
                    {
                        avoidance += diff.normalized * (penetration * avoidanceWeight);
                    }
                }
            }

            a.FinalVelocity = a.DesiredVelocity + avoidance;
        }
    }
}