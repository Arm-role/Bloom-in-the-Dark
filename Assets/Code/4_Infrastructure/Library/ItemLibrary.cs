using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemLibrary", menuName = "Library/ItemLibrary")]
public class ItemLibrary : ScriptableObject
{
    public List<ItemDefinition> Entries;

    public ItemDefinition Find(string friendlyName)
    {
        var entries = Entries.Find(l => friendlyName == l.Name);
        return entries;
    }
    public ItemDefinition Find(int id)
    {
        var entries = Entries.Find(l => id == l.ID);
        return entries;
    }
    public int FindIdByName(string friendlyName)
    {
        return Entries.Find(e => e.Name == friendlyName).ID;
    }
    public string FindNameById(int id)
    {
        return Entries.Find(e => e.ID == id).Name;
    }
}
