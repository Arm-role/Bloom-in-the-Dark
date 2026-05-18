using UnityEngine;

[CreateAssetMenu(menuName = "Grid/FenceRuleSet")]
public class FenceRuleSet : ScriptableObject, IFenceRuleSet
{
    [SerializeField] private Sprite[] _sprites = new Sprite[16];

    public Sprite GetSprite(int bitmask)
    {
        if (bitmask < 0 || bitmask >= _sprites.Length)
            return null;

        return _sprites[bitmask];
    }
}
