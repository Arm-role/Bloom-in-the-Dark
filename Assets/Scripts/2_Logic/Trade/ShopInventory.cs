using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trade/ShopInventory")]
public class ShopInventory : ScriptableObject
{
  [SerializeField] private List<TradeOffer> offers;

  public IReadOnlyList<TradeOffer> Offers => offers;
}
