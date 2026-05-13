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
    var pos = _rt.anchoredPosition;
    var bw  = background.rect.width;
    var bh  = background.rect.height;
    var hw  = canvasRect.rect.width  * 0.5f;
    var hh  = canvasRect.rect.height * 0.5f;

    if (pos.x + bw >  hw) pos.x =  hw - bw;
    if (pos.x      < -hw) pos.x = -hw;
    if (pos.y + bh >  hh) pos.y =  hh - bh;
    if (pos.y      < -hh) pos.y = -hh;

    _rt.anchoredPosition = pos;
  }
}
