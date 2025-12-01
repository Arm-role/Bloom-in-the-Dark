using UnityEngine;

[ExecuteAlways]
public class EnemyAIDebug : MonoBehaviour
{
    public EnemyMovement movement;
    public EnemyController controller;

    [Header("Debug Toggles")]
    public bool showForward = true;
    public bool showSteeringBlend = true;
    public bool showAvoidance = true;
    public bool showSeparation = true;
    public bool showGapProbes = true;
    public bool showColliderRadius = true;
    public bool showStateText = true;
    public bool showSkillRanges = true;
    public bool showSteeringField = false;

    [Header("Steering Field")]
    public int fieldResolution = 12;
    public float fieldCellSize = 0.5f;

    private Camera _cam;
}