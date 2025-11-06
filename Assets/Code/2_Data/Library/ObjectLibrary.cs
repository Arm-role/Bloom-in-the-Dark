using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Object/ObjectLibrary")]
public class ObjectLibrary : ScriptableObject
{
    public List<BaseObjectData> objects = new List<BaseObjectData>();

    public BaseObjectData GetObjectData(string id)
    {
        return objects.Find(o => o.Id == id);
    }
}