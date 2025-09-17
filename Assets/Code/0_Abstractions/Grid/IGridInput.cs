using System;
using UnityEngine;

public interface IGridInput
{
    Action OnPrimaryAction { get; set; }
    Vector3 MouseWorldPosition { get; }
}
