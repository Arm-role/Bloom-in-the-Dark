using UnityEngine;

public interface IDragGhost
{
    void Active();
    void UnActive();
    void Show(Sprite sprite, int amount);
    void Hide();

}
