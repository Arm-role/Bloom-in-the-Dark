using System.Collections.Generic;

public static class UpgradeRequestQuery
{
  public static bool TryGetRequestsUsingItem(
        List<ItemKey> items,
        List<UpgradeRequestDefinition> upgradeRequests,
        out List<UpgradeRequestDefinition> requests)
  {
    requests = new List<UpgradeRequestDefinition>();

    foreach (var upgrade in upgradeRequests)
    {
      bool match = true;

      foreach (var item in items)
      {
        bool found = false;

        foreach (var ingredient in upgrade.ingredients)
        {
          if (ingredient.item.Equals(item))
          {
            found = true;
            break;
          }
        }

        if (!found)
        {
          match = false;
          break;
        }
      }

      if (match)
        requests.Add(upgrade);
    }

    return requests.Count > 0;
  }
}