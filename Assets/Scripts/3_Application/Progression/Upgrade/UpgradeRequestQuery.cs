using System.Collections.Generic;

public static class UpgradeRequestQuery
{
  public static bool TryGetRequestsUsingItem(
        List<int> itemIds,
        List<UpgradeRequestDefinition> upgradeRequests,
        out List<UpgradeRequestDefinition> requests)
  {
    requests = new List<UpgradeRequestDefinition>();

    foreach (var upgrade in upgradeRequests)
    {
      bool match = true;

      foreach (var itemId in itemIds)
      {
        bool found = false;

        foreach (var ingredient in upgrade.Ingredients)
        {
          if (ingredient.item.RuntimeTag.Hash == itemId)
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