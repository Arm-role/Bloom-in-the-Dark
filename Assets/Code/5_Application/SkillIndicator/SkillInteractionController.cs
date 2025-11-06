using UnityEngine;

public class SkillInteractionController
{
    private readonly ParticalService _particalService;

    public SkillInteractionController(ParticalService particalService)
    {
        _particalService = particalService;
    }

    public void Initial()
    {

    }

    public void SetUp()
    {

    }
    public void ActiveSkill(string skillName, Vector2 targetPos)
    {
        _particalService.Play(skillName, targetPos);
    }
    public void ActiveSkill(string skillName, Vector2 targetPos, Vector2 direction)
    {
        _particalService.Play(skillName, targetPos, direction);
    }
}