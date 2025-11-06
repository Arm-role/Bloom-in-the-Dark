using System.Collections.Generic;
using UnityEngine;

public abstract class CombineLibraryBase<T1, T2> : ScriptableObject, ILibrary 
{
    public List<ObjectEntry<T1, T2>> Entries;

    public T1 Find1(string friendlyName)
    {
        var entries = Entries.Find(l => friendlyName == l.Name);
        return entries.Adressable1;
    }
    public T1 Find1(int id)
    {
        var entries = Entries.Find(l => id == l.Id);
        return entries.Adressable1;
    }
    public T2 Find2(string friendlyName)
    {
        var entries = Entries.Find(l => friendlyName == l.Name);
        return entries.Adressable2;
    }
    public T2 Find2(int id)
    {
        var entries = Entries.Find(l => id == l.Id);
        return entries.Adressable2;
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