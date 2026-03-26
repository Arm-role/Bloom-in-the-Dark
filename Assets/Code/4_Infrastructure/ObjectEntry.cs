using System;

[Serializable]
public class ObjectEntry
{
  public ObjectKey Key;
}
[Serializable]
public class ObjectEntry<T> : ObjectEntry
{
  public T Addressable;
}