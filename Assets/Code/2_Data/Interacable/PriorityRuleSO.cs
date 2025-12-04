using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/PriorityRule")]
public class PriorityRuleSO : ScriptableObject, IPriorityRule
{
    [System.Serializable]
    public class RuleEntry
    {
        public ETileBlockType Target;
        public float Modifier;
    }

    public float defaultPriority = 0;

    [SerializeField] private RuleEntry[] rules;

    public float Evaluate(ETileBlockType type)
    {
        foreach (var r in rules)
        {
            if ((type & r.Target) != 0)
                return r.Modifier;
        }
        return 0;
    }
}
