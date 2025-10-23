using System.Collections;
using UnityEngine;
public interface IItemAction
{
    void Setup();
    void Action(Vector2 placePoint);
}