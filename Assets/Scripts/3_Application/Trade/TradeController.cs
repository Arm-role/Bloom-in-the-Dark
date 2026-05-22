using System.Collections.Generic;

// orchestrator ของ trade — IGameSystem บน TradeState (Enter เปิด view / Exit ปิด)
public sealed class TradeController : IGameSystem
{
  private readonly PlayerInventory _inventory;
  private readonly ItemFactory _itemFactory;
  private readonly IItemDefinitionProvider _itemProvider;
  private readonly GameStateMachine _stateMachine;
  private readonly ITradeView _view;
  private readonly IPlayerInput _input;

  private ShopInventory _currentShop;

  public TradeController(
    PlayerInventory inventory,
    ItemFactory itemFactory,
    IItemDefinitionProvider itemProvider,
    GameStateMachine stateMachine,
    ITradeView view,
    IPlayerInput input)
  {
    _inventory = inventory;
    _itemFactory = itemFactory;
    _itemProvider = itemProvider;
    _stateMachine = stateMachine;
    _view = view;
    _input = input;
  }

  // เรียกจาก MerchantNpc.OnTradeRequested
  public void OpenTrade(ShopInventory shop)
  {
    if (shop == null) return;
    _currentShop = shop;
    _stateMachine.ChangeState(EGameState.Trade);
  }

  // ---- IGameSystem ----

  public void Enter()
  {
    _view.OnOfferClicked += HandleOfferClicked;
    // กด E ซ้ำตอนเปิดอยู่ = ปิด trade (subscribe ตอน Enter → ไม่โดน E ครั้งที่เปิด)
    _input.OnInteract += CloseTrade;
    _view.Open(BuildDisplay());
  }

  public void Exit()
  {
    _view.OnOfferClicked -= HandleOfferClicked;
    _input.OnInteract -= CloseTrade;
    _view.Close();
    _currentShop = null;
  }

  public void Update(float dt) { }
  public void FixedUpdate(float dt) { }

  // ---- internal ----

  private void CloseTrade()
    => _stateMachine.ChangeState(EGameState.Gameplay);

  private void HandleOfferClicked(int index)
  {
    if (_currentShop == null) return;

    var offers = _currentShop.Offers;
    if (index < 0 || index >= offers.Count) return;

    if (TryExecuteOffer(offers[index]))
      _view.Refresh(BuildDisplay());
  }

  private bool TryExecuteOffer(TradeOffer offer)
  {
    // เช็ค input ครบทุกตัวก่อน — กันหักครึ่งทางแล้วของไม่พอ
    foreach (var input in offer.Inputs)
    {
      var def = Resolve(input.item);
      if (def == null || !_inventory.CanRemoveItemAnywhere(def, input.amount))
        return false;
    }

    foreach (var input in offer.Inputs)
      _inventory.RemoveItemAnywhere(Resolve(input.item), input.amount);

    var outputDef = Resolve(offer.Output.item);
    if (outputDef != null)
      _inventory.AddItem(_itemFactory.Create(outputDef), offer.Output.amount);

    return true;
  }

  private List<TradeOfferDisplay> BuildDisplay()
  {
    var result = new List<TradeOfferDisplay>();
    if (_currentShop == null) return result;

    foreach (var offer in _currentShop.Offers)
      result.Add(ToDisplay(offer));

    return result;
  }

  private TradeOfferDisplay ToDisplay(TradeOffer offer)
  {
    var inputs = new TradeItemDisplay[offer.Inputs.Count];
    bool canAfford = true;

    for (int i = 0; i < offer.Inputs.Count; i++)
    {
      var ing = offer.Inputs[i];
      var def = Resolve(ing.item);
      int owned = def != null ? _inventory.CountItemAnywhere(def) : 0;

      inputs[i] = new TradeItemDisplay
      {
        ItemId = ing.item != null ? ing.item.RuntimeTag.Hash : 0,
        RequiredAmount = ing.amount,
        OwnedAmount = owned
      };

      if (owned < ing.amount)
        canAfford = false;
    }

    return new TradeOfferDisplay
    {
      Inputs = inputs,
      Output = new TradeItemDisplay
      {
        ItemId = offer.Output.item != null ? offer.Output.item.RuntimeTag.Hash : 0,
        RequiredAmount = offer.Output.amount,
        OwnedAmount = 0
      },
      CanAfford = canAfford
    };
  }

  private IItemDefinition Resolve(ItemKey key)
    => key == null ? null : _itemProvider.GetItem(key.RuntimeTag.Hash);
}
