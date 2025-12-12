using UnityEngine;

public class ExplosionEventController : MonoBehaviour, ISkillController<PlantItemInstance>, IPoolable<GameObject>
{
    public PlantExplosionSkill Skill;
    public float TriggerTime = 0.35f;

    private float timer;

    private bool isInitial;

    public bool IsAlive { get; set; }

    public void Initialze(PlantItemInstance plantItem, InteractionHandleContext ctx)
    {
        var yScale = Mathf.Cos(55 * Mathf.Deg2Rad);
        Skill = new PlantExplosionSkill(plantItem, ctx, yScale);
        isInitial = true;
    }

    public void OnReturnToPool(GameObject ob) => isInitial = false;

    public void OnSpawnFromPool(GameObject ob) { }

    void Update()
    {
        if (!isInitial) return;
        timer += Time.deltaTime;
        if (timer >= TriggerTime)
        {
            Skill.Cast(transform.position);
            enabled = false;
        }
    }
}
