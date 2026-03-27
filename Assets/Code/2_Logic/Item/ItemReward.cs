public struct ItemReward
{
    public string ItemID;
    public int Count;

    public ItemReward(string itemID, int count)
    {
        ItemID = itemID;
        Count = count;
    }
}