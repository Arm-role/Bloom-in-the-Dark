using System.Collections.Generic;
using System;
using UnityEngine;

public class MockSkillController : MonoBehaviour
{
    [Serializable]
    public class SkillSlot
    {
        public KeyCode key;
        public float cooldown;
        [HideInInspector] public float lastCastTime;
        public PlantItem plantItem;
    }

    public List<SkillSlot> skills = new List<SkillSlot>();

    private void Update()
    {
        foreach (var slot in skills)
        {
            if (Input.GetKeyDown(slot.key))
                TryCast(slot);
        }
    }

    private void TryCast(SkillSlot slot)
    {
        var yScale = Mathf.Cos(55 * Mathf.Deg2Rad);
        var plant = new PlantItemInstance(slot.plantItem);
        //var skill = new PlantExplosionSkill(plant, yScale);

        // cooldown
        if (Time.time < slot.lastCastTime + slot.cooldown)
            return;

        // world position of mouse
        Vector2 castPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // cast
        //skill.Cast(castPosition);

        slot.lastCastTime = Time.time;
    }
}
