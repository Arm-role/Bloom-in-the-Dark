using UnityEngine;

public class PlantGrowthController : MonoBehaviour, IGrowthEntity
{
    [SerializeField] private int turnsPerStage = 1;
    [SerializeField] private PlantItem plantItem;

    private int turnCounter = 0;

    private WorldInteractable _interactable;
    private PlantState _state;
    private ITurnView _view;

    private void OnEnable()
    {
        _interactable = GetComponent<WorldInteractable>();
        _state = GetComponent<PlantState>();
        _view = GetComponent<ITurnView>();

        _interactable.OnRequestDestruction += ResetTileStage;
        _view.UpdateVisual(_state.GrowthStage);
    }

    private void OnDisable()
    {
        _interactable.OnRequestDestruction -= ResetTileStage;
    }

    public void OnTurnPassed()
    {
        if (_state.IsGrown) return;

        turnCounter++;

        if (turnCounter >= turnsPerStage)
        {
            turnCounter = 0;
            _state.Grow();
            _view.UpdateVisual(_state.GrowthStage);
        }
    }

    private void ResetTileStage(GameObject obj)
    {
        _state.ResetStage();
        _view.UpdateVisual(_state.GrowthStage);
    }
}
