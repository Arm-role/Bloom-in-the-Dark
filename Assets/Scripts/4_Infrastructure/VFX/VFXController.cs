using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour, IVFXService
{
  public float de = 1;
  public VisualEffect fogRender;

  private float _radius;

  private void Update()
  {
    float scale = fogRender.transform.lossyScale.x;
    fogRender.SetFloat("ColliderRadius", (_radius - de) / scale);
  }

  public void ApplyFog(float radius)
  {
    _radius = radius;
  }

  private void OnDrawGizmos()
  {
    float scale = fogRender.transform.lossyScale.x;
    Gizmos.DrawWireSphere(fogRender.transform.position, (_radius - de) / scale);
  }
}