#nullable enable
using System.Collections;
using UnityEngine;

public sealed class FlashHitView : MonoBehaviour, IFlashHitView
{
    // Cached once at class load — string→ID lookup happens just on first JIT.
    private static readonly int FlashID = Shader.PropertyToID("_Flash");

    [SerializeField] private float duration;
    [SerializeField] private SpriteRenderer[] m_renderers = null!;

    // MaterialPropertyBlock keeps the shared material — SRP batcher can batch
    // all sprites together. Using renderer.material would instance the material
    // per renderer and break batching.
    private readonly MaterialPropertyBlock _mpb = new();

    public void SetObject() => SetFlash(0);

    public void FlashEffect() => StartCoroutine(FlashRoutine());

    private IEnumerator FlashRoutine()
    {
        SetFlash(1);
        yield return new WaitForSeconds(duration);
        SetFlash(0);
    }

    private void SetFlash(int value)
    {
        for (int i = 0; i < m_renderers.Length; i++)
        {
            var renderer = m_renderers[i];
            if (renderer == null) continue;

            renderer.GetPropertyBlock(_mpb);
            _mpb.SetInt(FlashID, value);
            renderer.SetPropertyBlock(_mpb);
        }
    }
}
