using UnityEngine;

public class AreaCirclePreview : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite rangeSprite;
    [SerializeField] private Sprite healSprite;
    [SerializeField] private string sortingLayer;

    [Header("Visual Settings")]
    [SerializeField] private Color rangeColor = new(0f, 1f, 0f, 0.25f);
    [SerializeField] private Color healColor = new(0f, 0.75f, 1f, 0.4f);

    private GameObject _rangeGO;
    private GameObject _targetGO;
    private SpriteRenderer _rangeRenderer;
    private SpriteRenderer _healRenderer;

    public void Initialize()
    {
        // สร้าง GameObject แสดงผล
        _rangeGO = CreateIndicator("RangeIndicator", rangeSprite, rangeColor, 100);
        _targetGO = CreateIndicator("TargetIndicator", healSprite, healColor, 101);
        Disable();
    }

    public void UpdateView(Vector2 rangePos, Vector2 healPos, Vector3 rangeScale, Vector3 healScale)
    {
        _rangeGO.transform.position = rangePos;
        _targetGO.transform.position = healPos;

        _rangeGO.transform.localScale = rangeScale;
        _targetGO.transform.localScale = healScale;
    }

    public void Enable()
    {
        _rangeGO.SetActive(true);
        _targetGO.SetActive(true);
    }

    public void Disable()
    {
        if (_rangeGO) _rangeGO.SetActive(false);
        if (_targetGO) _targetGO.SetActive(false);
    }

    private GameObject CreateIndicator(string name, Sprite sprite, Color color, int sortingOrder)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform);
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        renderer.sortingLayerName = sortingLayer;
        renderer.sortingOrder = sortingOrder;
        if (name.Contains("Range")) _rangeRenderer = renderer;
        else _healRenderer = renderer;
        return go;
    }
}