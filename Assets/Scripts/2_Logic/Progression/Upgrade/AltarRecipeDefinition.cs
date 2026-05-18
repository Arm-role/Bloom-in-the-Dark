using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Altar/Recipe")]
public class AltarRecipeDefinition : ScriptableObject
{
  public const int MaxSlots = 6;

  [SerializeField] private List<Ingredient> ingredients;
  [SerializeField] private ItemKey resultItem;
  [SerializeField] private ObjectKey resultNpc;
  [SerializeField] private Vector2 npcSpawnOffset;
  [SerializeField] private Sprite previewIcon;

  public string Name            => name;
  public ItemKey ResultItem     => resultItem;
  public int ResultItemId       => resultItem.RuntimeTag.Hash;
  public bool IsNpcCraft        => resultNpc != null;
  public ObjectKey ResultNpc    => resultNpc;
  public int ResultNpcId        => resultNpc.RuntimeTag.Hash;
  public Vector2 NpcSpawnOffset => npcSpawnOffset;
  public Sprite PreviewIcon     => previewIcon;
  public IReadOnlyList<Ingredient> Ingredients => ingredients;

  public Sprite GetPreviewIcon(IItemIconProvider itemIconProvider)
      => previewIcon != null ? previewIcon : itemIconProvider.GetIcon(ResultItemId);

  public int TotalSlots => ingredients?.Sum(i => i.amount) ?? 0;

#if UNITY_EDITOR
  private void OnValidate()
  {
    int total = TotalSlots;
    if (total > MaxSlots)
      Debug.LogWarning($"[{name}] Total ingredient slots ({total}) exceeds altar max ({MaxSlots}).", this);

    if (resultItem != null && resultNpc != null)
      Debug.LogWarning($"[{name}] Both resultItem and resultNpc are set — only one will be used (NPC takes priority).", this);

    if (resultItem == null && resultNpc == null)
      Debug.LogWarning($"[{name}] No result defined — set either resultItem or resultNpc.", this);

    if (resultNpc != null && previewIcon == null)
      Debug.LogWarning($"[{name}] NPC recipe has no previewIcon — assign a sprite or the preview UI will be blank.", this);
  }
#endif
}

[Serializable]
public struct Ingredient
{
  public ItemKey item;
  [Range(1, AltarRecipeDefinition.MaxSlots)] public int amount;
}
