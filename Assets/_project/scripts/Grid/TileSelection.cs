using _project.scripts.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _project.scripts.grid
{
    public class TileSelection : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap, _obstacleTilemap;
        [SerializeField] private float _offset = 0.5f;
        [SerializeField] private Vector2 _gridSize = new Vector2(1f, 1f);

        private Vector2Int HighlightedTilePosition {  get; set; } = Vector2Int.zero;
        public Vector2 WorldHighlightedTilePosition => GridUtils.GridToWorld(HighlightedTilePosition);

        private void Update()
        {
            // TODO : to remove
            if (Helper.Camera == null) return;
            
            Vector3 mouseWorldPos = Helper.Camera.ScreenToWorldPoint(Helper.MousePosition);

            Vector2Int gridPos = new Vector2Int(
                Mathf.FloorToInt(mouseWorldPos.x / _gridSize.x) * Mathf.RoundToInt(_gridSize.x),
                Mathf.FloorToInt(mouseWorldPos.y / _gridSize.y) * Mathf.RoundToInt(_gridSize.y)
            );

            bool isObstacleTile = false;

            if (_obstacleTilemap != null)
            {
                Vector3Int cellPos = _obstacleTilemap.WorldToCell(mouseWorldPos);
                if (_obstacleTilemap.HasTile(cellPos) && _obstacleTilemap.GetTile(cellPos) is not null)
                {
                    isObstacleTile = true;
                }
            }

            if (!isObstacleTile)
            {
                HighlightedTilePosition = gridPos;
                Vector2 worldPos = GridUtils.GridToWorld(gridPos) + new Vector2(_offset, _offset);
                transform.position = worldPos;
            }
        }
    }
}