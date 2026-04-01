using UnityEngine;

public class CycleGizmoDrawer : MonoBehaviour
{
  [SerializeField] private CycleData cycleData;
  [SerializeField] private int previewPoints = 15;

  private void OnDrawGizmos()
  {
    if (cycleData == null || cycleData.Waves == null) return;

    foreach (var wave in cycleData.Waves)
    {
      DrawWave(wave);
    }
  }

  private void DrawWave(WaveDefinition wave)
  {
    if (wave == null) return;

    Color color = GetWaveColor(wave.Type);

    if (wave.NormalSpawn != null)
      DrawPattern(wave.NormalSpawn, color);

    if (wave.EndBurstSpawn != null)
      DrawPattern(wave.EndBurstSpawn, Color.red);
  }

  private void DrawPattern(SpawnPattern pattern, Color baseColor)
  {
    Vector3 origin = transform.position;

    // Outer radius
    Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.25f);
    Gizmos.DrawWireSphere(origin, pattern.MaxRadius);

    // Inner radius
    Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.5f);
    Gizmos.DrawWireSphere(origin, pattern.MinRadius);

    // Preview spawn points
    Gizmos.color = baseColor;

    Random.InitState(12345); // deterministic preview

    for (int i = 0; i < previewPoints; i++)
    {
      float min = pattern.MinRadius;
      float max = pattern.MaxRadius;

      float radius = Mathf.Sqrt(Random.Range(min * min, max * max));
      float angle = Random.Range(0f, Mathf.PI * 2f);

      Vector3 pos = origin + new Vector3(
          Mathf.Cos(angle) * radius,
          Mathf.Sin(angle) * radius,
          0f
      );

      Gizmos.DrawSphere(pos, 0.15f);
    }
  }

  private Color GetWaveColor(WaveType type)
  {
    switch (type)
    {
      case WaveType.Normal: return Color.green;
      case WaveType.Burst: return Color.yellow;
      case WaveType.Single: return Color.cyan;
      default: return Color.white;
    }
  }
}