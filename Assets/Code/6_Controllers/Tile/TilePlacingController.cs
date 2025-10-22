using UnityEngine;

public class TilePlacingController : MonoBehaviour
{
    // --- Dependencies (Inject by Installer) ---
    private IPlayerInput _playerInput;
    private WorldGridModel _gridModel;
    private GridLogic _gridLogic;
    private IPlayerStateService _playerStateService;

    public void Initialize(IPlayerInput input, WorldGridModel model, GridLogic gridLogic, IPlayerStateService playerStateService)
    {
        _playerInput = input;
        _gridModel = model;
        _gridLogic = gridLogic;
        _playerStateService = playerStateService;
    }

    private void Update()
    {
        // ... (โค้ด Preview การวาง Tile) ...

        if (_playerInput.IsPrimaryActionDown)
        {
            // 1. ถาม Service ว่าผู้เล่นถือ Tile อะไรอยู่
            string heldTileID = _playerStateService.GetCurrentHeldTileID();

            // 2. ถ้าถืออยู่ ถึงจะสั่ง Model
            if (!string.IsNullOrEmpty(heldTileID))
            {
                Vector2Int gridPos = _gridLogic.WorldToGrid(_playerInput.PointerWorldPosition);
                _gridModel.PlaceTile(gridPos, heldTileID);
            }
        }

        if (_playerInput.IsSecondaryActionDown)
        {
            Vector2Int gridPos = _gridLogic.WorldToGrid(_playerInput.PointerWorldPosition);
            _gridModel.RemoveTile(gridPos);
        }
    }
}
