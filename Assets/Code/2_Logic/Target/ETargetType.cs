[System.Flags]
public enum ETargetType
{
    None = 0,
    Enemy = 1 << 0,
    Ally = 1 << 1,
    Interactable = 1 << 2,
    Item = 1 << 3,
    Ground = 1 << 4,

    All = ~0
}
