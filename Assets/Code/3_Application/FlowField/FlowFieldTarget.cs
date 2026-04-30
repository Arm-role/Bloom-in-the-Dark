using UnityEngine;

public class FlowFieldTarget : MonoBehaviour
{
  [SerializeField] private FlowFieldChannelKey flowKey;
  [SerializeField] private float baseThreat;
  [SerializeField] private bool isObjectiveTarget;
  [SerializeField] private Vector2Int size;
  public FlowFieldChannelKey FlowKey => flowKey;
  public float BaseThreat => baseThreat;
  public bool IsObjectiveTarget => isObjectiveTarget;
}