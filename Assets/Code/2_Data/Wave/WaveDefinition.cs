using System;
using UnityEngine;

[Serializable]
public class WaveDefinition
{
    public WaveType Type;

    public float Duration;
    [Range(0f, 1f)]
    public float NextWaveStartRatio;

    [Header("Normal / Single Spawn")]
    public SpawnPattern NormalSpawn;

    [Header("End Burst Spawn")]
    public SpawnPattern EndBurstSpawn;

    [Tooltip("Used only when Type = Single")]
    public bool SpawnAtStart = true;
}