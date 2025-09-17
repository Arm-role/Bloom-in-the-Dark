using UnityEngine;

public class DragDropController : MonoBehaviour
{
    private IDrag _currentState;
    private Vector2 _startDragPosition;
    private float _holdTimer;
    private bool _hasMovedTooMuch;

    [Header("Drag Settings")]
    [SerializeField] private float _holdThreshold = 0.5f;
    [SerializeField] private float _holdMoveTolerance = 0.5f;

    // --- Dependencies ---
    private IPlayerInput _playerInput;
    private InteractionService _interactionService;
    private IItemInstance _draggedItem; 

    public void Initialize(IPlayerInput playerInput, InteractionService interactionService,float holdThreshold, float holdMoveTolerance)
    {
        _playerInput = playerInput;
        _interactionService = interactionService;

        _holdThreshold = holdThreshold;
        _holdMoveTolerance = holdMoveTolerance;
        this.enabled = false; // เริ่มต้นด้วยการ "หลับ"
    }
}