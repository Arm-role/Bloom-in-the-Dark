using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/PriorityRule")]
public class PriorityRuleSO : ScriptableObject, IPriorityRule
{
    [System.Serializable]
    public class RuleEntry
    {
        public EWorldInteractableType Target;
        public float Modifier;
    }

    public float defaultPriority = 0;

    [SerializeField] private RuleEntry[] rules;

    public float Evaluate(EWorldInteractableType type)
    {
        foreach (var r in rules)
        {
            if ((type & r.Target) != 0)
                return r.Modifier;
        }
        return 0;
    }
}
