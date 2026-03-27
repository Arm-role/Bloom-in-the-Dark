using UnityEngine;

public interface IPlayerState
{
    void UpdateMoveDirection(Vector2 input);
    void Look(Vector2 lookDir);
}
