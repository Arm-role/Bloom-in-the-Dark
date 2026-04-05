using UnityEngine;

public class NavigationDebugController : MonoBehaviour
{
  public EnemyController enemy;

  public Transform targetA;
  public Transform targetB;
  public Transform targetC;

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      enemy.AssignTarget(targetA);  
    }

    if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      enemy.AssignTarget(targetB);
    }

    if (Input.GetKeyDown(KeyCode.Alpha3))
    {
      enemy.AssignTarget(targetC);
    }
  }
}