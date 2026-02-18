using UnityEngine;

public class PlantGrowthController : MonoBehaviour, IGrowthEntity
{
    [SerializeField] private int turnsPerStage = 1;
    [SerializeField] private PlantSkillCasterItem plantSkillCasterItem;

    private int turnCounter = 0;

    private IDestructible _destructible;
    private PlantState _state;
    private ITurnView _view;

    private void OnEnable()
    {
        _destructible = GetComponent<IDestructible>();
        _state = GetComponent<PlantState>();
        _view = GetComponent<ITurnView>();

        _destructible.OnRequestDestruction += ResetTileStage;
        _view.UpdateVisual(_state.GrowthStage);
    }

    private void OnDisable()
    {
        _destructible.OnRequestDestruction -= ResetTileStage;
    }

    public void OnTurnPassed(ETurnState turnState)
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
