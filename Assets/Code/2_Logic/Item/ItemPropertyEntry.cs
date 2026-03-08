using UnityEngine;

[System.Serializable]
public struct ItemPropertyEntry
{
    public EItemProperty Property;

    public int IntValue;
    public float FloatValue;
    public string StringValue;
    public Vector2Int Vector2IntValue;
}
