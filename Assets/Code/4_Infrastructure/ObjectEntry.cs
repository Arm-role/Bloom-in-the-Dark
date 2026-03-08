using System;

[Serializable]
public class ObjectEntry
{
    public string Name;
    public int Id;
}
[Serializable]
public class ObjectEntry<T> : ObjectEntry
{
    public T Adressable;
}
[Serializable]
public class ObjectEntry<T1, T2> : ObjectEntry
{
    public T1 Adressable1;
    public T2 Adressable2;
}
