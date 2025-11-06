using System.Collections.Generic;
using UnityEngine;

public class ProximityDetector
{
    private float _range = 1.5f;
    private bool _isActive;
    private GameObject _previewObj;

    public void Setup(float range)
    {
        _range = range;
    }

    public ProximityInteractionData DetectTargets(Vector2 center)
    {
        //Collider2D[] hits = Physics2D.OverlapCircleAll(center, _range);
        //List<Collider2D> validTargets = new List<Collider2D>();

        //foreach (var hit in hits)
        //{
        //    if (hit.CompareTag("Enemy") || hit.CompareTag("Interactable"))
        //    {
        //        validTargets.Add(hit);
        //    }
        //}

        //return new ProximityInteractionData
        //{
        //    Center = center,
        //    Targets = validTargets
        //};

        return null;
    }

    public void EnablePreview(Vector2 center)
    {
        _isActive = true;
        ShowPreview(center);
    }

    public void UpdatePreview(Vector2 center)
    {
        if (!_isActive) return;
        UpdatePreviewPosition(center);
    }

    public void DisablePreview()
    {
        _isActive = false;
        HidePreview();
    }

    // --- Gizmo / Visual ---
    private void ShowPreview(Vector2 pos)
    {
        //if (!_previewObj)
        //{
        //    _previewObj = GameObject.CreatePrimitive(PrimitiveType.Circle);
        //    _previewObj.transform.localScale = new Vector3(_range * 2, _range * 2, 1);
        //    _previewObj.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.2f);
        //}
        //_previewObj.transform.position = pos;
        //_previewObj.SetActive(true);
    }

    private void UpdatePreviewPosition(Vector2 pos)
    {
        if (_previewObj)
            _previewObj.transform.position = pos;
    }

    private void HidePreview()
    {
        if (_previewObj)
            _previewObj.SetActive(false);
    }
}