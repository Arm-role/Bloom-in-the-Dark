using UnityEngine;
using UnityEngine.Rendering;

public class PlantView : MonoBehaviour, IObjectTurnView
{
    [SerializeField] private SortingGroup _sortingGroup;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite[] stageSprites;

    public void UpdateVisual(int stage)
    {
        _sortingGroup.sortingLayerName = (stage > 0) ? "Foreground" : "Background";
        _sortingGroup.sortingOrder = (stage > 0) ? 0 : 10;
        _renderer.sprite = stageSprites[Mathf.Clamp(stage, 0, stageSprites.Length - 1)];
    }
}
