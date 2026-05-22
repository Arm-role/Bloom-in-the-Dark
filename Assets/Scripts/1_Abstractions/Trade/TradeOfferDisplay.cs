// view-model — ข้อมูล 1 trade offer สำหรับ TradeView render
public struct TradeOfferDisplay
{
  public TradeItemDisplay[] Inputs;
  public TradeItemDisplay Output;
  public bool CanAfford;
}

public struct TradeItemDisplay
{
  public int ItemId;
  public int RequiredAmount;
  public int OwnedAmount;
}
