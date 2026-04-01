using System;

[Flags]
public enum InteractionPhase
{
    None = 0,
    Pressed = 1 << 0,
    Held = 1 << 1,
    Released = 1 << 2
}