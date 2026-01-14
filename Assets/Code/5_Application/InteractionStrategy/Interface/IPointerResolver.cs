using System.Collections.Generic;
using UnityEngine;

public interface IPointerResolver
{
    void Setup(InteractionHandleContext context);
    IEnumerable<PointerResolveResult> Resolve(
        InteractionHandleContext context);
}