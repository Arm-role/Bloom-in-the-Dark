using UnityEngine;

[CreateAssetMenu(fileName = "CycleData", menuName = "Mock/CycleData")]
public class CycleData : ScriptableObject
{
    public WaveDefinition[] Waves;
}
public enum WaveType
{
    Normal,
    Burst,
    Single
}