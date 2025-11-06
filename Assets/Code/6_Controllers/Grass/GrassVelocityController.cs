using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassVelocityController : MonoBehaviour
{
    [Range(0f,1f)] public float ExternInfluenceStrength = 0.25f;
    public float EaseInTime = 0.15f;
    public float EaseOutTime = 0.15f;
    public float VelocityThreshold = 5;

    [Header("Spring Settings")]
    [Range(0f, 5f)] public float SpringStiffness = 2.5f;
    [Range(0f, 2f)] public float SpringDamping = 0.5f;

    public AnimationCurve EaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private int _externInfluence = Shader.PropertyToID("_ExternInfluence");
    public void InfluenceGrass(Material mat, float XVelocity)
    {
        mat.SetFloat(_externInfluence, XVelocity);
    }
}
