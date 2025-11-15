using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "View/MousePreview")]
public class MousePreviewData : ScriptableObject
{
    [System.Serializable]
    public class MousePreview
    {
        public Sprite MouseSprite;
        public EWorldInteractableType WorldType;
    }

    [SerializeField] private MousePreview[] previews;

    public Dictionary<EWorldInteractableType, Sprite> targets;

    public void Initialize()
    {
        targets = new();
        foreach (var m in previews)
        {
            if (m.WorldType != EWorldInteractableType.None && !targets.ContainsKey(m.WorldType))
                targets.Add(m.WorldType, m.MouseSprite);
        }
    }
}
