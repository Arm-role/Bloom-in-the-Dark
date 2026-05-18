using System;

[Serializable]
public class PreviewRule
{
    public InputActionType Input; // None = ไม่ต้องกด
    public InteractionPhase PhaseMask; // ใช้ได้กับ interaction
    public EItemStrategyType Strategy;

    public ItemSelectionPhase SelectionPhase; 

    public PreviewAction Action;
}