using UnityEngine;
using System.Collections.Generic;

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

    void OnDrawGizmos()
    {
        if (_combat == null) _combat = GetComponent<EnemyCombat>();
        if (_combat == null) return;

        var skills = _combat.GetSkills();
        if (skills == null) return;

        foreach (var s in skills)
        {
            if (s == null) continue;

            // draw based on runtime type
            if (s is MeleeSkill ms)
                DrawMeleeSkillGizmos(ms);
            else if (s is DashSkill ds)
                DrawDashSkillGizmos(ds);
            else
                DrawGenericSkillGizmos(s);
        }
    }

    void DrawMeleeSkillGizmos(MeleeSkill ms)
    {
        Vector3 pos = transform.position;
        float range = ms.Range;

        // --- 1) draw full reach disk ---

        HandlesColorPush(meleeFill);
        UnityEditor.Handles.DrawSolidDisc(pos, Vector3.forward, range);
        HandlesColorPop();

        Gizmos.color = meleeOutline;
        Gizmos.DrawWireSphere(pos, range);


        // --- 2) draw the actual attack OverlapCircle (the one used in code) ---
        float hitRadius = 0.5f;
        Vector3 attackDir = transform.right; // approximate forward direction
        Vector3 hitCenter = pos + attackDir.normalized * range;

        Color hitFill = new Color(1f, 0.4f, 0.1f, 0.12f);
        Color hitOutline = new Color(1f, 0.4f, 0.1f, 1f);

        HandlesColorPush(hitFill);
        UnityEditor.Handles.DrawSolidDisc(hitCenter, Vector3.forward, hitRadius);
        HandlesColorPop();

        Gizmos.color = hitOutline;
        Gizmos.DrawWireSphere(hitCenter, hitRadius);

        // arrow direction
        DrawArrowGizmo(pos, hitCenter, meleeOutline);

#if UNITY_EDITOR
        UnityEditor.Handles.color = textColor;
        UnityEditor.Handles.Label(hitCenter + Vector3.up * 0.15f,
            $"Overlap: r={hitRadius:0.00}");
        UnityEditor.Handles.Label(pos + Vector3.up * 0.35f,
            $"Melee: R={range:0.00} CD={ms.Cooldown:0.00}");
#endif
    }

    void DrawDashSkillGizmos(DashSkill ds)
    {
        Vector3 pos = transform.position;
        float maxRange = ds.MaxRange;
        float dashSpeed = GetPrivateFloat(ds, "_speed", ds.MaxRange / Mathf.Max(0.0001f, ds.Cooldown)); // fallback

        // circle showing max dash reach
        HandlesColorPush(dashFill);
        UnityEditor.Handles.DrawSolidDisc(pos, Vector3.forward, maxRange);
        HandlesColorPop();

        Gizmos.color = dashOutline;
        Gizmos.DrawWireSphere(pos, maxRange);

        // small arrow forward showing "example" direction (can't know target dir here)
        Vector3 fwd = transform.right; // approximate facing (depends on your facing system)
        Vector3 end = pos + fwd * Mathf.Min(maxRange, 1.5f);
        DrawArrowGizmo(pos, end, dashOutline);

#if UNITY_EDITOR
        UnityEditor.Handles.color = textColor;
        UnityEditor.Handles.Label(pos + Vector3.up * 0.25f, $"Dash: maxR={maxRange:0.00} dur={GetPrivateFloat(ds, "_duration", 0f):0.00}");
#endif
    }

    void DrawGenericSkillGizmos(IEnemySkill s)
    {
        Vector3 pos = transform.position;
#if UNITY_EDITOR
        UnityEditor.Handles.color = textColor;
        UnityEditor.Handles.Label(pos + Vector3.up * 0.25f, $"{s.GetType().Name} R={s.MaxRange:0.00} CD={s.Cooldown:0.00}");
#endif
    }

    // small helper: draw arrow
    void DrawArrowGizmo(Vector3 a, Vector3 b, Color col)
    {
        Gizmos.color = col;
        Gizmos.DrawLine(a, b);
        Vector3 dir = (b - a).normalized;
        float head = 0.15f;
        Vector3 right = Quaternion.Euler(0, 0, 160f) * dir * head;
        Vector3 left = Quaternion.Euler(0, 0, -160f) * dir * head;
        Gizmos.DrawLine(b, b + right);
        Gizmos.DrawLine(b, b + left);
    }

    // read private field helper (defensive)
    float GetPrivateFloat(object obj, string fieldName, float fallback = 0f)
    {
        if (obj == null) return fallback;
        var t = obj.GetType();
        var fi = t.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        if (fi == null) return fallback;
        object v = fi.GetValue(obj);
        if (v is float f) return f;
        return fallback;
    }

    // small helpers to set Handles.color safely
    void HandlesColorPush(Color c) { UnityEditor.Handles.color = c; }
    void HandlesColorPop() { UnityEditor.Handles.color = Color.white; }
}