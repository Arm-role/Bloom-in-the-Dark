
using UnityEngine;
public interface ICharacterAnimationView
{
    void SetMoveDirection(Vector2 moveDirection);
    void SetLookirection(Vector2 lookDirection);

    void PlayAnimation(string key);
    void PlayAttack(string attackKey);
    void PlayDash();
    void PlaySlam();
    void PlayHit();
    void PlayDeath();
}