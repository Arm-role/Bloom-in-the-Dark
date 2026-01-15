using System;

[Flags]
public enum InteractionCondition
{
    None = 0,
    RequireSecondaryHeld = 1 << 0,
    RequireNoSecondaryHeld = 1 << 1,
    RequirePrimaryHeld = 1 << 2,
    RequireNoSkillPreviewActive = 1 << 3,
}