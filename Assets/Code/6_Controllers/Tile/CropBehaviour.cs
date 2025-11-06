using NUnit.Framework.Interfaces;
using System;
using UnityEngine;

public class CropBehaviour : MonoBehaviour
{
    public string cropId;
    public int growthStage = 0;
    public float growthTimer = 0f;

    [Serializable]
    public class SpriteAndRenderer
    {
        public Sprite sprite;
        public SpriteRenderer spriteRenderer;
    }

    public SpriteAndRenderer[] growthStages;

    public Action<CropBehaviour> OnRequestDestruction;
    public void Initialize(string id)
    {
        cropId = id;
        growthStage = 0;
        UpdateVisual(growthStage);
    }
    private void Start()
    {
        UpdateVisual(growthStage);
    }

    private void Update()
    {
        growthTimer += Time.deltaTime;

        if (growthTimer >= 5f)
        {
            growthTimer = 0f;
            growthStage = Mathf.Min(growthStage + 1, growthStages.Length - 1);
            UpdateVisual(growthStage);
        }
    }

    public void UpdateVisual(int growthStage)
    {
        if (growthStages.Length < growthStage) return;

        var sprite = growthStages[growthStage].sprite;
        var renderer = growthStages[growthStage].spriteRenderer;

        if (renderer != null)
        {
            foreach (var stage in growthStages)
            {
                if (stage.spriteRenderer == renderer)
                {
                    if (renderer.sprite == sprite) continue;

                    renderer.sprite = sprite;
                    stage.spriteRenderer.gameObject.SetActive(true);
                }
                else
                {
                    stage.spriteRenderer.gameObject.SetActive(false);
                }
            }
        }
    }

    public void Harvest()
    {
        Debug.Log($"Harvested {cropId}");
        RequestDestruction();
    }

    public void RequestDestruction() => OnRequestDestruction?.Invoke(this);
}
