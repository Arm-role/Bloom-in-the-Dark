using System;

[Flags]
public enum EItemType
{
    None = 0,

    Seed = 1 << 0,     
    Plant = 1 << 1,       
    Tool = 1 << 2,       
    Resource = 1 << 3,     
    Building = 1 << 4,       

    All = ~0
}