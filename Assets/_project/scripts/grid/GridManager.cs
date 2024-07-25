using UnityEngine;

namespace _project.scripts.grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private int _width, _height;
        [SerializeField] private Tile _tilePrefab;
        void NewGrid()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    
                }
            }
            
        }
    }
}