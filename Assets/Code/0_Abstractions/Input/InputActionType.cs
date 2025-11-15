[System.Flags]
public enum InputActionType
{
    None = 0,
    Primary = 1 << 0,
    Secondary = 1 << 1,
    SkillModifierHeld = 1 << 2,
    Special1 = 1 << 3,
    Special2 = 1 << 4
}