using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _project.scripts.grid
{
    public class ObstacleTileMap : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        private HashSet<Vector3Int> _obstacleTilePosition = new HashSet<Vector3Int>();

        private void Awake()
        {
            if (_tilemap == null)
            {
                _tilemap = GetComponent<Tilemap>();
            }

            InitializeObstacleTiles();
        }

        private void InitializeObstacleTiles()
        {
            _obstacleTilePosition.Clear();

            BoundsInt bounds = _tilemap.cellBounds;
            TileBase[] allTiles = _tilemap.GetTilesBlock(bounds);

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    if (tile != null)
                    {
                        Vector3Int position = new Vector3Int(bounds.x + x, bounds.y + y, 0);
                        _obstacleTilePosition.Add(position);
                    }
                }
            }
        }

        public bool IsTileObstacle(Vector2 position) => _obstacleTilePosition.Contains(_tilemap.WorldToCell(position));
    }
}