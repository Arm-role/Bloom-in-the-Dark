using System.Collections;
using UnityEngine;

public class FlashHitView : MonoBehaviour, IFlashHitView
{
    [SerializeField]
    private float duration;

    [SerializeField]
    private SpriteRenderer m_renderer;

    public void SetObject()
    {
        m_renderer.material.SetInt("_Flash", 0);
    }

    public void FlashEffect()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        m_renderer.material.SetInt("_Flash", 1);
        yield return new WaitForSeconds(duration);
        m_renderer.material.SetInt("_Flash", 0);
    }
}
