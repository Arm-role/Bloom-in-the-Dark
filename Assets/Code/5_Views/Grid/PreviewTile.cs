using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PreviewTile : MonoBehaviour, IPreviewTile
{
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Show(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
        gameObject.SetActive(true);
    }

    public void SetPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }
}
