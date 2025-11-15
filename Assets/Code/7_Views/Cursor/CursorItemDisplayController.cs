using UnityEngine;
using UnityEngine.UI;

public class CursorItemDisplayController : MonoBehaviour, ICursorItemDisplay
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Vector2 _offset = new(32, -32);

    private Camera _camera;
    private bool _isVisible = false;

    private void Awake()
    {
        _camera = Camera.main;
        Hide();
    }

    private void Update()
    {
        if (!_isVisible) return;

        Vector2 mousePos = Input.mousePosition;
        _itemIcon.rectTransform.position = mousePos + _offset;
    }

    public void Show(Sprite icon)
    {
        _itemIcon.sprite = icon;
        _itemIcon.enabled = true;
        _isVisible = true;
    }

    public void Hide()
    {
        _itemIcon.enabled = false;
        _isVisible = false;
    }

    public void SetColor(Color color)
    {
        _itemIcon.color = color;
    }
}