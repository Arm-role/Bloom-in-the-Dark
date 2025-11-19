using System;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public event Action OnNextTurn;

    public int TurnCount { get; private set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextTurn();
        }
    }
    public void NextTurn()
    {
        TurnCount++;
        OnNextTurn?.Invoke();
    }
}