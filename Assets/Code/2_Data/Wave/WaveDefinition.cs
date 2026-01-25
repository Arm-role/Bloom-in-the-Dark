using System;

[Serializable]
public class WaveDefinition
{
    public float Duration;       // ความยาว wave
    public float NextWaveStartRatio;
    public SpawnPattern NormalSpawn;
    public SpawnPattern EndBurstSpawn;      // ช่วงท้าย wave
    public float EndBurstStartTime;         // เช่น 80% ของ wave
}