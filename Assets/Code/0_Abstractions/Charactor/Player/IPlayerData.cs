using UnityEngine;

public interface IPlayerData
{
    void UpdateMoveDirection(Vector2 input);
    void Look(Vector2 lookDir);
}
