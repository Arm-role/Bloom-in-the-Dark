public readonly struct ItemCategoryData
{
  public readonly EItemCategory Category;
  public readonly EItemRole ItemRole;

  public ItemCategoryData(
    EItemCategory category,
    EItemRole itemRole)
  {
    Category = category;
    ItemRole = itemRole;
  }

  public static ItemCategoryData None()
    => new ItemCategoryData(EItemCategory.None, EItemRole.None);
}