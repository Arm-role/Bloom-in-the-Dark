using System;
using System.Collections.Generic;

public interface ITradeView
{
  event Action<int> OnOfferClicked;

  void Open(IReadOnlyList<TradeOfferDisplay> offers);
  void Refresh(IReadOnlyList<TradeOfferDisplay> offers);
  void Close();
}
