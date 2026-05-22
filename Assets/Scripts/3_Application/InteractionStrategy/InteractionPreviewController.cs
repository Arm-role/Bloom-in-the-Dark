#nullable enable
using System;

// Owns the interaction preview indicator: resolves the preview bundle for the
// selected item and keeps it shown/updated/hidden. Driven by ItemInteractionAction.
public sealed class InteractionPreviewController
{
  private readonly InteractionHandleService _handleService;
  private readonly Func<InputActionType, InteractionHandleContext> _createContext;

  private IPreviewProvider? _provider;
  private ItemStrategyBundle? _bundle;

  public InteractionPreviewController(
    InteractionHandleService handleService,
    Func<InputActionType, InteractionHandleContext> createContext)
  {
    _handleService = handleService;
    _createContext = createContext;
  }

  // true เมื่อ preview กำลังแสดง — resolver ใช้เช็ค IsSkillPreviewActive
  public bool IsActive => _bundle != null;

  // เรียกเมื่อ item ที่เลือกเปลี่ยน — รีเซ็ต preview ให้ตรงกับ item ใหม่
  public void SetProvider(IPreviewProvider? provider)
  {
    Disable();
    _provider = provider;
    Tick();
  }

  public void Handle(InputActionType input, InteractionPhase phase)
  {
    if (input == InputActionType.None || _provider == null)
      return;

    var ctx = _createContext(input);

    foreach (var rule in _provider.GetPreviewRules(input, phase, ItemSelectionPhase.Selected))
      ApplyRule(rule, ctx);
  }

  public void Tick()
  {
    if (_provider == null)
      return;

    var ctx = _createContext(InputActionType.None);

    foreach (var rule in _provider.GetPreviewRules(
               InputActionType.None,
               InteractionPhase.None,
               ItemSelectionPhase.Selected))
      ApplyRule(rule, ctx);
  }

  public void Disable()
  {
    _bundle?.Preview?.Hide();
    _bundle = null;
  }

  private void ApplyRule(PreviewRule rule, InteractionHandleContext ctx)
  {
    if (rule.Action == PreviewAction.Disable)
    {
      Disable();
      return;
    }

    var bundle = _handleService.Resolve(rule.Strategy);
    if (bundle == null)
      return;

    if (_bundle != bundle)
    {
      Disable();
      _bundle = bundle;
      bundle.Preview?.Setup();
    }

    var config = bundle.Targeting.ConfigProvider.Create(ctx);
    var target = bundle.Targeting.Strategy.Resolve(ctx, config);
    bundle.Preview?.Update(target);
  }
}
