
public class DirectInteractValidator : ITargetValidator
{
    public bool CanInteract(IDataProvider data)
    {
        if (data is not DirectInteractData directInteractData) return false;
        if (directInteractData.PointerPosition.Value == null) return false;

        if (directInteractData.Target.IsTile)
        {
            var tileState = directInteractData.Target.TileState;
            if (tileState != null && !tileState.IsOccupied)
            {
                return true;
            }
        }

        return false;
    }
}