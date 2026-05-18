using System.Collections;
using UnityEngine;

public class FlashHitView : MonoBehaviour, IFlashHitView
{
    [SerializeField]
    private float duration;

    [SerializeField]
    private SpriteRenderer[] m_renderers;

    public void SetObject()
    {
        SetInt("_Flash", 0);
    }

    public void FlashEffect()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        SetInt("_Flash", 1);
        yield return new WaitForSeconds(duration);
        SetInt("_Flash", 0);
    }

    private void SetInt(string name, int value)
    {
        foreach (var renderer in m_renderers)
        {
            renderer.material.SetInt(name, value);
        }
    }
}
