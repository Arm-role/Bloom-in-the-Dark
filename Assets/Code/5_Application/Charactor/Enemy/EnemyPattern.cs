using System.Collections;
using UnityEngine;

public abstract class EnemyPattern : ScriptableObject
{
    public abstract IEnumerator Run(EnemyController enemy, Transform target);
}
