using UnityEngine;

[ExecuteAlways]
public class SkillGizmosDrawer : MonoBehaviour
{
    public Color meleeFill = new Color(1f, 0.2f, 0.2f, 0.08f);
    public Color meleeOutline = new Color(1f, 0.2f, 0.2f, 1f);

    public Color dashFill = new Color(0.2f, 0.6f, 1f, 0.06f);
    public Color dashOutline = new Color(0.2f, 0.6f, 1f, 1f);

    public Color textColor = Color.white;

    EnemyCombat _combat;

    void Awake() => _combat = GetComponent<EnemyCombat>();
}