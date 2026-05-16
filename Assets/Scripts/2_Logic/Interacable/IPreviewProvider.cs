using System.Collections.Generic;

public interface IPreviewProvider
{
    IEnumerable<PreviewRule> GetPreviewRules(
        InputActionType input,
        InteractionPhase phase,
        ItemSelectionPhase selection);
}
