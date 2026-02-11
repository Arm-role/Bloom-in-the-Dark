using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour
{
    [SerializeField] private ETurnState defaultTurnState;
    [Header("UI")]
    [SerializeField] private Button nextTurnButton;
    [SerializeField] private TextMeshProUGUI textTurnState;

    public event Action<ETurnState> OnNextTurn;
    private ETurnState _turnState;

    private PlayerController Pcon;
    private CycleController CycleController;

    public void Initialize(
        PlayerController pcon, 
        CycleController cycleController)
    {
        Pcon = pcon;
        CycleController = cycleController;
        
        nextTurnButton.onClick.AddListener(NextTurn);
        nextTurnButton.gameObject.SetActive(false);

        Pcon.PlayerEnergy.OnChanged += OnCurrentEnergyChanged;
        CycleController.OnCycleCompleted += OnBattleCycleCompleted;

        SetTurn(defaultTurnState);
    }

    private void OnDisable()
    {
        Pcon.PlayerEnergy.OnChanged -= OnCurrentEnergyChanged;
        CycleController.OnCycleCompleted -= OnBattleCycleCompleted;
    }

    private void OnCurrentEnergyChanged(ResourceChangedEvent e)
    {
        if (e.Current <= 5)
            nextTurnButton.gameObject.SetActive(true);
    }

    private void OnBattleCycleCompleted()
    {
        NextTurn();
    }

    private void NextTurn()
    {
        switch (_turnState)
        {
            case ETurnState.Farm:
                SetTurn(ETurnState.Preparation);
                break;

            case ETurnState.Preparation:
                SetTurn(ETurnState.Battle);
                break;

            case ETurnState.Battle:
                SetTurn(ETurnState.Farm);
                break;
        }
    }

    private void SetTurn(ETurnState newState)
    {
        _turnState = newState;
        textTurnState.text = _turnState.ToString();
        OnNextTurn?.Invoke(_turnState);

        HandleTurnEnter(newState);
    }

    private void HandleTurnEnter(ETurnState state)
    {
        switch (state)
        {
            case ETurnState.Battle:
                nextTurnButton.gameObject.SetActive(false);
                CycleController.StartCycle();
                break;
            
            case ETurnState.Farm:
                nextTurnButton.gameObject.SetActive(false);
                Pcon.PlayerEnergy.ReFill();
                break;

            default:
                CycleController.StopCycle();
                break;
        }
    }
}
