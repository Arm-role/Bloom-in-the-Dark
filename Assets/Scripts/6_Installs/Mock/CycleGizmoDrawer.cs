using UnityEngine;

public class CycleGizmoDrawer : MonoBehaviour
{
  [SerializeField] private CycleData cycleData;
  [SerializeField] private int previewPoints = 15;
  [SerializeField] private bool DrawGizmo;

  private void OnDrawGizmos()
  {
    if (!DrawGizmo) return;
    if (cycleData == null || cycleData.Wave == null) return;

    DrawWave(cycleData.Wave);
  }

  private void DrawWave(WaveDefinition wave)
  {
    if (wave == null || wave.spawn == null) return;
    DrawPattern(wave.spawn, Color.green);
  }

  private void DrawPattern(SpawnPattern pattern, Color baseColor)
  {
    Vector3 origin = transform.position;

    Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.25f);
    Gizmos.DrawWireSphere(origin, pattern.MaxRadius);

    Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f);
    Gizmos.DrawWireSphere(origin, pattern.MinRadius);

    Gizmos.color = baseColor;

    Random.InitState(12345);

    for (int i = 0; i < previewPoints; i++)
    {
      float min = pattern.MinRadius;
      float max = pattern.MaxRadius;

      float radius = Mathf.Sqrt(Random.Range(min * min, max * max));
      float angle = Random.Range(0f, Mathf.PI * 2f);

      Vector3 pos = origin + new Vector3(
          Mathf.Cos(angle) * radius,
          Mathf.Sin(angle) * radius,
          0f);

      Gizmos.DrawSphere(pos, 0.15f);
    }
  }
}
