using System;
using UnityEngine;

public class ExplosionEventController : 
    MonoBehaviour, 
    ISkillController,
    IPoolable<GameObject>,
    IDestructible
{
    public AreaCircleSkill Skill;
    public float TriggerTime = 0.35f;

    private float timer;

    private bool isInitial;

    public bool IsAlive { get; set; }
    public event Action<GameObject> OnRequestDestruction;

    public void Initialze(IItemInstance itemInstance, InteractionHandleContext ctx)
    {
        var yScale = Mathf.Cos(55 * Mathf.Deg2Rad);
        Skill = new AreaCircleSkill(itemInstance, ctx, yScale);
        
        isInitial = true;
        timer = 0;
    }

    public void OnReturnToPool(GameObject ob)
    {
        isInitial = false;
        timer = 0;
    }

    public void OnSpawnFromPool(GameObject ob) { }

    void Update()
    {
        if (!isInitial) return;
        timer += Time.deltaTime;
        if (timer >= TriggerTime)
        {
            Skill.Cast(transform.position);
            RequestDestruction();
        }
    }

    public void RequestDestruction()
    {
        OnRequestDestruction?.Invoke(gameObject);
    }
}
