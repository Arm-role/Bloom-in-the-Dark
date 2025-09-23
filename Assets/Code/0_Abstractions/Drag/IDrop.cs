using System;
using UnityEngine;

public interface IDrop
{
    DropExecutionResult Execute(IItemInstance itemInstance);
}