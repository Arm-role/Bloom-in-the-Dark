using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "View/MousePreview")]
public class MousePreviewData : ScriptableObject
{
    [System.Serializable]
    public class MousePreview
    {
        public Sprite MouseSprite;
        public ETileCapability WorldType;
    }

    [SerializeField] private MousePreview[] previews;

    public Dictionary<ETileCapability, Sprite> targets;

    public void Initialize()
    {
        targets = new();
        foreach (var m in previews)
        {
            if (m.WorldType != ETileCapability.None && !targets.ContainsKey(m.WorldType))
                targets.Add(m.WorldType, m.MouseSprite);
        }
    }
}
