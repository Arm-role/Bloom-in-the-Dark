using UnityEngine;

public class LineAttackEventController : MonoBehaviour, ISkillController, IPoolable<GameObject>
{
    public LineMeleeSkill Skill;
    public float TriggerTime = 0.15f; // melee speedเร็วกว่า plant

    private float timer;
    private bool isInitial;

    public bool IsAlive { get; set; }

    public void Initialze(IItemInstance itemInstance, InteractionHandleContext ctx)
    {
        Skill = new LineMeleeSkill(itemInstance, ctx);
        timer = 0;
        isInitial = true;
        enabled = true;
    }

    public void OnReturnToPool(GameObject ob)
    {
        isInitial = false;
        timer = 0;
    }

    public void OnSpawnFromPool(GameObject ob)
    {
        // nothing
    }

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
