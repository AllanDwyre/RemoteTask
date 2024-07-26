using System;
using _project.scripts.utils;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using Debug = System.Diagnostics.Debug;

namespace _project.scripts.grid
{
    // TODO : - Better way to deal with obstacle (do we really need a tilemap for obstacle, what are the limits of it ? )
    
    public class TileSelection : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap, _obstacleTilemap;
        [SerializeField] private float _offset = 0.5f;
        [SerializeField] private Vector2 _gridSize = new Vector2(1f, 1f);

        public Vector2Int HighlightedTilePosition {  get; private set; } = Vector2Int.zero;
        public Vector2 WorldHighlightedTilePosition => GridUtils.GridToWorld(HighlightedTilePosition);

        public bool IsHighlightedTileClicked(Vector2 clickedPosition)
        {
            return GridUtils.WorldToGrid(clickedPosition) == HighlightedTilePosition;
        }
        public bool IsTileObstacle(Vector2Int position)
        {
            RaycastHit2D hit = Physics2D.Raycast(GridUtils.GridToWorld(position), Vector2.zero);
            return hit.collider;
        }
        private void Update()
        {
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