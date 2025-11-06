using System.Collections.Generic;
using UnityEngine;

public abstract class LibraryBase<T> : ScriptableObject
{
    public List<ObjectEntry<T>> Entries;

    public T Find(string friendlyName)
    {
        var entries = Entries.Find(l => friendlyName == l.Name);
        return entries.Adressable;
    }
    public T Find(int id)
    {
        var entries = Entries.Find(l => id == l.Id);
        return entries.Adressable;
    }
    public int FindIdByName(string friendlyName)
    {
        return Entries.Find(e => e.Name == friendlyName).Id;
    }
    public string FindNameById(int id)
    {
        return Entries.Find(e => e.Id == id).Name;
    }
}
