using UnityEngine;

public struct SteeringResult
{
    public Vector2 desiredDir;
    public float speedMul;
    public float priorityWeight;

    public SteeringResult(Vector2 dir, float mul = 1f, float weight = 1f)
    {
        desiredDir = dir;
        speedMul = mul;
        priorityWeight = weight;
    }

    public static SteeringResult Zero => new SteeringResult(Vector2.zero, 0f, 0f);
}