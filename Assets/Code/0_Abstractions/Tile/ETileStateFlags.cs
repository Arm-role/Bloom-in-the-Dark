[System.Flags]
public enum ETileStateFlags
{
    None = 0,
    Tilled = 1 << 0,
    Watered = 1 << 1,
    Fertilized = 1 << 2,
    Planted = 1 << 3,
    Blocked = 1 << 4,
}