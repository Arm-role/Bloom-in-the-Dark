using UnityEngine;
using UnityEngine.UI;

public class DragGhost : MonoBehaviour, IDragGlost
{
    public Image Icon;
    public CanvasGroup CanvasGroup;

    public void Active() => gameObject.SetActive(true);
    public void UnActive() => gameObject.SetActive(false);

    public void Show(Sprite sprite)
    {
        Icon.sprite = sprite;
        CanvasGroup.alpha = 1;
    }

    public void Hide()
    {
        CanvasGroup.alpha = 0;
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }
}