using UnityEngine;

public static class CombatDistanceUtility
{
  public static float EdgeDistance(
      Transform a,
      float radiusA,
      Transform b,
      float radiusB)
  {
    float centerDist =
        Vector2.Distance(
            a.position,
            b.position
        );

    return centerDist
        - radiusA
        - radiusB;
  }
}