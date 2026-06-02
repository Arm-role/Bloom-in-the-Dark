using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipView : MonoBehaviour, ITooltipView
{
  [SerializeField] private RectTransform canvasRect;
  [SerializeField] private RectTransform background;
  [SerializeField] private TMP_Text titleText;
  [SerializeField] private TMP_Text descriptionText;
  [SerializeField] private Vector2 offset = new Vector2(12f, -12f);

  private RectTransform _rt;

  private void Awake()
  {
    _rt = GetComponent<RectTransform>();
    Hide();
  }

  private void Update()
  {
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        (RectTransform)transform.parent,
        Input.mousePosition,
        null,
        out var localPoint);
    _rt.localPosition = localPoint + offset;
    Clamp();
  }

  public void Show(TooltipData data)
  {
    gameObject.SetActive(true);
    transform.SetAsLastSibling();
    titleText.text       = data.Title;
    descriptionText.text = data.Description;
    LayoutRebuilder.ForceRebuildLayoutImmediate(background);
    LayoutRebuilder.ForceRebuildLayoutImmediate(background);
    Update();
  }

  public void Hide() => gameObject.SetActive(false);

  private void Clamp()
  {
    // Measure the visible box in canvas space so the clamp is independent of
    // the tooltip's pivot/anchor — a hard-coded (0,0)-pivot assumption let the
    // box slide past the bottom/right edge when the prefab used a top-left pivot.
    var canvasBounds = canvasRect.rect;
    var box = RectTransformUtility.CalculateRelativeRectTransformBounds(canvasRect, background);

    var shift = Vector2.zero;

    var overRight = box.max.x - canvasBounds.xMax;
    if (overRight > 0f) shift.x -= overRight;
    var overLeft = box.min.x - canvasBounds.xMin;
    if (overLeft < 0f) shift.x -= overLeft;   // left wins if box wider than canvas → text start stays visible

    var overBottom = box.min.y - canvasBounds.yMin;
    if (overBottom < 0f) shift.y -= overBottom;
    var overTop = box.max.y - canvasBounds.yMax;
    if (overTop > 0f) shift.y -= overTop;       // top wins if box taller than canvas → title stays visible

    _rt.anchoredPosition += shift;
  }
}
