using UnityEngine;

public class AreaCircleIndicatorPreview : MonoBehaviour, IAreaCircleIndicatorPreview
{
    [Header("Sprites")]
    [SerializeField] private Sprite rangeSprite;
    [SerializeField] private Sprite healSprite;
    [SerializeField] private string sortingLayer;

    [Header("Visual Settings")]
    [SerializeField] private Color rangeColor = new(0f, 1f, 0f, 0.25f);
    [SerializeField] private Color healColor = new(0f, 0.75f, 1f, 0.4f);

    [Header("Debug")]
    [SerializeField] private bool _drawGizmo;

    private GameObject _rangeGO;
    private GameObject _targetGO;
    private SpriteRenderer _rangeRenderer;
    private SpriteRenderer _healRenderer;

    public void Initialize()
    {
        // สร้างครั้งเดียว — Setup() ถูกเรียกซ้ำทุกครั้งที่เปลี่ยน strategy
        // ถ้าไม่ guard GameObject เก่าจะค้างสะสมใต้ transform เรื่อยๆ
        if (_rangeGO != null) return;

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

#if UNITY_EDITOR
    // วาดวงตาม transform จริงของ indicator — เทียบกับ sprite ใน prefab ว่า pivot/ขนาดตรงไหม
    private void OnDrawGizmos()
    {
        if (!_drawGizmo) return;

        if (_rangeGO != null && _rangeGO.activeSelf)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = _rangeGO.transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        }

        if (_targetGO != null && _targetGO.activeSelf)
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = _targetGO.transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        }
    }
#endif
}
