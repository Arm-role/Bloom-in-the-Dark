using UnityEngine;
using UnityEngine.UI;

public class DragGhost : MonoBehaviour ,IDragGlost
{
    public Image Icon;
    public CanvasGroup CanvasGroup;

    public void Show(Sprite sprite)
    {
        Icon.sprite = sprite;
        CanvasGroup.alpha = 1;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        CanvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }
}