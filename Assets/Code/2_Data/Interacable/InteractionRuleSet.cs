using UnityEngine;

[CreateAssetMenu(menuName = "Game/Interaction Rule Set")]
public class InteractionRuleSet : ScriptableObject
{
    [System.Serializable]
    public class InteractionRule
    {
        public EItemType ItemType;
        public EWorldInteractableType WorldType;
        public bool CanInteract;
    }

    [SerializeField] private InteractionRule[] rules;

    public bool CanInteract(EItemType itemType, EWorldInteractableType worldType)
    {
        foreach (var rule in rules)
        {
            bool worldMatch = (rule.WorldType & worldType) != 0;

            bool itemMatch = (rule.ItemType & itemType) != 0;

            if (worldMatch && itemMatch)
            {
                return rule.CanInteract;
            }
        }

        return false;
    }
}
