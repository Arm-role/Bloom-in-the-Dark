using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragGhost : MonoBehaviour, IDragGhost
{
    public Image Icon;
    public TextMeshProUGUI TextAmount;
    public CanvasGroup CanvasGroup;

    private RectTransform _rt;
    private RectTransform _parentRect;
    private Camera _uiCamera;

    private void Awake()
    {
        _rt = (RectTransform)transform;
        _parentRect = transform.parent as RectTransform;

        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            _uiCamera = canvas.worldCamera;
    }

    public void Active() => gameObject.SetActive(true);
    public void UnActive() => gameObject.SetActive(false);

    public void Show(Sprite sprite, int amount)
    {
        Icon.sprite = sprite;
        if (amount <= 1)
            TextAmount.text = string.Empty;
        else
            TextAmount.text = amount.ToString();
        CanvasGroup.alpha = 1;
    }

    public void Hide()
    {
        CanvasGroup.alpha = 0;
    }

    private void Update()
    {
        // Convert mouse screen point to the canvas' local space so the ghost tracks
        // the cursor under any canvas render mode (Overlay / Camera / World), not just
        // Screen Space - Overlay where screen point == world position.
        if (_parentRect == null)
        {
            transform.position = Input.mousePosition;
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentRect, Input.mousePosition, _uiCamera, out var localPoint))
            _rt.localPosition = localPoint;
    }
}