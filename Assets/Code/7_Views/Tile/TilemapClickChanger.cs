using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapClickChanger : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase newTile; // tile ที่จะเปลี่ยนเป็น

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = tilemap.WorldToCell(worldPos);

            TileBase currentTile = tilemap.GetTile(cellPos);
            Debug.Log($"Clicked Tile at {cellPos}, current: {currentTile}");

            // toggle tile: ถ้ามี tile อยู่แล้ว → เอาออก / ถ้าไม่มี → ใส่
            if (currentTile == null)
            {
                tilemap.SetTile(cellPos, newTile);
            }
            else
            {
                tilemap.SetTile(cellPos, null);
            }
        }
    }
}
