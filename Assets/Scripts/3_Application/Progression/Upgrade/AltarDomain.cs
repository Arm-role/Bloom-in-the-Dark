using System;
using System.Collections.Generic;
using UnityEngine;

public class AltarDomain
{
  private EAltarMode _mode = EAltarMode.None;
  private IItemDefinition _buffItem;
  private IItemDefinition _lockedPlant;
  private int _placedCount;
  private AltarRecipeDefinition _pendingCraft;

  private readonly GameTag _moonBloomTag;
  private readonly GameTag _plantTag;
  private readonly AltarRecipeDatabase _recipeDatabase;
  private readonly Func<int> _getRequiredCount;
  private readonly Dictionary<int, int> _craftContainer = new();

  public event Action<IItemDefinition, bool> OnUpgradeReady;
  public event Action<List<AltarRecipeDefinition>> OnRecipeSuggestionsChanged;
  public event Action<AltarRecipeDefinition> OnCraftPreviewReady;
  public event Action<AltarRecipeDefinition> OnCraftReady;
  public event Action OnCleared;

  public AltarDomain(
    AltarRecipeDatabase recipeDatabase,
    GameTag moonBloomTag,
    GameTag plantTag,
    Func<int> getRequiredCount)
  {
    _recipeDatabase = recipeDatabase;
    _moonBloomTag = moonBloomTag;
    _plantTag = plantTag;
    _getRequiredCount = getRequiredCount;
  }

  public bool PlaceItem(IItemDefinition item)
  {
    return _mode switch
    {
      EAltarMode.None        => HandleFirstItem(item),
      EAltarMode.UpgradeBuff => HandleBuffSecond(item),
      EAltarMode.Upgrade     => HandleUpgradePlant(item),
      EAltarMode.Craft       => HandleCraft(item),
      _                      => false,
    };
  }

  public void RemoveItem(IItemDefinition item)
  {
    switch (_mode)
    {
      case EAltarMode.Upgrade:
      case EAltarMode.UpgradeBuff:
        RemoveFromUpgradeMode(item);
        break;
      case EAltarMode.Craft:
        RemoveFromCraftMode(item);
        break;
    }
  }

  public void ConfirmCraft()
  {
    if (_pendingCraft == null) return;
    var confirmed = _pendingCraft;
    _pendingCraft = null;
    OnCraftReady?.Invoke(confirmed);
    Clear();
  }

  // =============================
  // Place Handlers
  // =============================

  private bool HandleFirstItem(IItemDefinition item)
  {
    if (item.HasTag(_moonBloomTag))
    {
      _mode = EAltarMode.UpgradeBuff;
      _buffItem = item;
    }
    else if (item.HasTag(_plantTag))
    {
      _mode = EAltarMode.Upgrade;
      AddPlant(item);
    }
    else
    {
      _mode = EAltarMode.Craft;
      if (!HandleCraft(item))
      {
        _mode = EAltarMode.None;
        return false;
      }
    }
    return true;
  }

  private bool HandleBuffSecond(IItemDefinition item)
  {
    if (!item.HasTag(_plantTag)) return false;
    AddPlant(item);
    return true;
  }

  private bool HandleUpgradePlant(IItemDefinition item)
  {
    if (!item.HasTag(_plantTag)) return false;
    AddPlant(item);
    return true;
  }

  private int TotalCraftSlots()
  {
    int total = 0;
    foreach (var count in _craftContainer.Values)
      total += count;
    return total;
  }

  private bool HandleCraft(IItemDefinition item)
  {
    if (TotalCraftSlots() >= AltarRecipeDefinition.MaxSlots) return false;

    if (!_craftContainer.ContainsKey(item.ID))
      _craftContainer[item.ID] = 0;
    _craftContainer[item.ID]++;

    FireCraftResult();
    return true;
  }

  // =============================
  // Remove Handlers
  // =============================

  private void RemoveFromUpgradeMode(IItemDefinition item)
  {
    if (item.HasTag(_moonBloomTag))
    {
      _buffItem = null;
      if (_placedCount == 0)
        Clear();
      else
        _mode = EAltarMode.Upgrade;
      return;
    }

    if (_lockedPlant == null || item.ID != _lockedPlant.ID) return;

    _placedCount = Mathf.Max(0, _placedCount - 1);

    if (_placedCount == 0)
    {
      _lockedPlant = null;
      if (_buffItem == null)
        Clear();
    }
  }

  private void RemoveFromCraftMode(IItemDefinition item)
  {
    if (!_craftContainer.ContainsKey(item.ID)) return;

    _craftContainer[item.ID]--;
    if (_craftContainer[item.ID] <= 0)
      _craftContainer.Remove(item.ID);

    _pendingCraft = null;

    if (_craftContainer.Count == 0)
    {
      Clear();
      return;
    }

    FireCraftResult();
  }

  // =============================
  // Upgrade Count
  // =============================

  private void AddPlant(IItemDefinition plant)
  {
    if (_lockedPlant == null)
      _lockedPlant = plant;

    _placedCount++;
    int required = _getRequiredCount();

    if (_placedCount >= required)
      FireUpgrade(_lockedPlant, _mode == EAltarMode.UpgradeBuff);
  }

  public bool HasPendingCraft => _pendingCraft != null;

  public IReadOnlyDictionary<int, int> GetCraftSnapshot() => _craftContainer;

  // =============================
  // Suggestions
  // =============================

  private AltarRecipeDefinition FindBestMatch()
  {
    AltarRecipeDefinition best = null;
    var recipes = _recipeDatabase?.recipes;
    foreach (var recipe in recipes ?? System.Linq.Enumerable.Empty<AltarRecipeDefinition>())
    {
      if (recipe == null) continue;
      if (!IsCraftMatch(recipe)) continue;
      if (best == null || recipe.TotalSlots > best.TotalSlots)
        best = recipe;
    }
    return best;
  }

  // Fire preview + partial suggestions when a recipe matches,
  // or clear preview + show full suggestions when nothing matches.
  // Suggestions fire first so HandleSuggestionsChanged runs before HandleCraftPreview,
  // keeping HideCraftPreview a no-op when the preview is about to open immediately after.
  private void FireCraftResult()
  {
    var best = FindBestMatch();
    if (best != null)
    {
      _pendingCraft = best;
      OnRecipeSuggestionsChanged?.Invoke(ComputeSuggestions(exclude: best));
      OnCraftPreviewReady?.Invoke(best);
    }
    else
    {
      _pendingCraft = null;
      FireSuggestions();
    }
  }

  private void FireSuggestions()
  {
    OnRecipeSuggestionsChanged?.Invoke(ComputeSuggestions());
  }

  private List<AltarRecipeDefinition> ComputeSuggestions(AltarRecipeDefinition exclude = null)
  {
    var recipes = _recipeDatabase?.recipes;
    if (recipes == null || _craftContainer.Count == 0)
      return new List<AltarRecipeDefinition>();

    UnityEngine.Debug.Log($"[AltarDomain] Placed: {string.Join(", ", System.Linq.Enumerable.Select(_craftContainer, kv => $"id={kv.Key} x{kv.Value}"))}");

    var result = new List<AltarRecipeDefinition>();
    foreach (var recipe in recipes)
    {
      if (recipe == null || recipe == exclude) continue;
      if (RecipeAcceptsAllPlaced(recipe))
        result.Add(recipe);
    }

    result.Sort((a, b) =>
    {
      int cmp = MatchScore(b).CompareTo(MatchScore(a)); // desc
      if (cmp != 0) return cmp;
      return a.TotalSlots.CompareTo(b.TotalSlots);      // asc
    });

    if (result.Count > 4)
      result.RemoveRange(4, result.Count - 4);

    UnityEngine.Debug.Log($"[AltarDomain] Suggestions ({result.Count}): {string.Join(", ", System.Linq.Enumerable.Select(result, r => $"{r.name}(score={MatchScore(r):F2})"))}");
    return result;
  }

  // Recipe is a candidate only if every placed item appears as an ingredient
  // and its placed quantity does not exceed the recipe's required amount.
  private bool RecipeAcceptsAllPlaced(AltarRecipeDefinition recipe)
  {
    foreach (var kv in _craftContainer)
    {
      bool found = false;
      foreach (var ingredient in recipe.Ingredients)
      {
        if (ingredient.item.RuntimeTag.Hash == kv.Key)
        {
          if (kv.Value > ingredient.amount) return false;
          found = true;
          break;
        }
      }
      if (!found) return false;
    }
    return true;
  }

  // Ratio of placed slots that satisfy recipe requirements (0.0–1.0).
  private float MatchScore(AltarRecipeDefinition recipe)
  {
    if (recipe.TotalSlots == 0) return 0f;
    int matched = 0;
    foreach (var ingredient in recipe.Ingredients)
    {
      _craftContainer.TryGetValue(ingredient.item.RuntimeTag.Hash, out int placed);
      matched += Mathf.Min(placed, ingredient.amount);
    }
    return (float)matched / recipe.TotalSlots;
  }

  // =============================
  // Helpers
  // =============================

  private void FireUpgrade(IItemDefinition plant, bool buffed)
  {
    OnUpgradeReady?.Invoke(plant, buffed);
    Clear();
  }

  private bool IsCraftMatch(AltarRecipeDefinition recipe)
  {
    foreach (var ingredient in recipe.Ingredients)
    {
      if (!_craftContainer.TryGetValue(ingredient.item.RuntimeTag.Hash, out var count))
        return false;
      if (count < ingredient.amount)
        return false;
    }
    return true;
  }

  private void Clear()
  {
    _mode = EAltarMode.None;
    _buffItem = null;
    _lockedPlant = null;
    _placedCount = 0;
    _pendingCraft = null;
    _craftContainer.Clear();
    OnRecipeSuggestionsChanged?.Invoke(new List<AltarRecipeDefinition>());
    OnCleared?.Invoke();
  }
}
