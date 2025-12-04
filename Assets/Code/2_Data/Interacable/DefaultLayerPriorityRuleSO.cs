using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/PriorityRuleDefault")]
public class DefaultLayerPriorityRuleSO : ScriptableObject, IPriorityRule
{
    [System.Serializable]
    public struct LayerPriority
    {
        public ETileBlockType Type;
        public float Priority;
    }

    public LayerPriority[] Priorities;

    public float Evaluate(ETileBlockType type)
    {
        foreach (var p in Priorities)
        {
            if (type.HasFlag(p.Type))
                return p.Priority;
        }
        return 0;
    }
}