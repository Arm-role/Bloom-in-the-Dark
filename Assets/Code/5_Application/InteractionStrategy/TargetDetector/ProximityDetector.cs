using UnityEngine;

public class ProximityDetector : ITargetDetector
{
    private readonly ProximityPointerResolver _resolver;
    private readonly InteractionDispatcher _dispatcher;

    public ProximityDetector(ProximityPointerResolver resolver, InteractionDispatcher dispatcher)
    {
        _resolver = resolver;
        _dispatcher = dispatcher;
    }

    public void Setup(InteractionHandleContext context)
    {
        _resolver.Setup(context);
    }

    public IDataProvider IntercationExcute(InteractionHandleContext context)
    {
        var pointer = _resolver.Resolve(context);
        var center = pointer.ResolvedPointer;

        //ToolItem tool = context.ItemInstance.Data as ToolItem;
        //float radius = tool.AttackRange;

        //// ตรวจหาวัตถุในระยะ
        //Collider2D hit = Physics2D.OverlapCircle(center, radius, LayerMask.GetMask("Interactable"));

        //_dispatcher.TryInteract(context, hit.transform.position, ETargetResolveType.Ground, out var target);

        //if (hit != null)
        //{
        //    if (_dispatcher.TryInteract(context, hit.transform.position, ETargetResolveType.Object, out var target))
        //    {
        //        return new ProximityInteractionData(context.ItemInstance, center, target);
        //    }
        //}

        return new ProximityInteractionData();
    }
}
