using UnityEngine;

namespace _project.scripts.grid
{
    public static class GridUtils
    {
        public static Vector2Int WorldToGrid(Vector2 worldPos)
        {
            return new(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y));
        }
        public static Vector2 GridToWorld(Vector2Int gridPos)
        {
            return new(gridPos.x, gridPos.y);
        }
    }
}
