using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragGhost : MonoBehaviour, IDragGhost
{
    public Image Icon;
    public TextMeshProUGUI TextAmount;
    public CanvasGroup CanvasGroup;

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
        transform.position = Input.mousePosition;
    }
}