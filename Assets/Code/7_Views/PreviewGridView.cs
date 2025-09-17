using System.Collections.Generic;
using UnityEngine;

public class PreviewGridView : MonoBehaviour, IPreviewGridView
{
    private List<PreviewTile> _tiles = new List<PreviewTile>();

    public void AddTile(GameObject instance)
    {
        if (instance.TryGetComponent<PreviewTile>(out var tile))
        {
            _tiles.Add(tile);
            tile.transform.SetParent(this.transform);
        }
    }

    public void UpdateTile(int index, Vector3 position, Color color)
    {
        if (index < 0 || index >= _tiles.Count) return;

        _tiles[index].gameObject.SetActive(true);
        _tiles[index].SetPosition(position);
        _tiles[index].SetColor(color);
    }

    public void HideAllTiles()
    {
        foreach (var tile in _tiles)
        {
            tile.gameObject.SetActive(false);
        }
    }
}
