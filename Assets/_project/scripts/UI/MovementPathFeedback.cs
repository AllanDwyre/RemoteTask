using System.Collections.Generic;
using System.Linq;
using _project.scripts.Characters;
using UnityEngine;

namespace _project.scripts.UI
{
    public class MovementPathFeedback : MonoBehaviour
    {
        private Vector3[] _path;
        private LineRenderer _line;
        [SerializeField] private MovementComponent movement;
        private void Start()
        {
            _line = GetComponent<LineRenderer>();
            if (movement == null)
            {
                Debug.LogError("MovementPathFeedback don't have MovementComponent specified");
                Destroy(this);
            }
        }
        private void OnEnable()
        {
            movement.OnNewPath += OnNewPath;
            movement.OnMoveAlongPath += MoveAlongPath;
        }
        private void OnDisable()
        {
            movement.OnNewPath -= OnNewPath;
            movement.OnMoveAlongPath -= MoveAlongPath;
        }
        private void OnNewPath(List<Vector2> path)
        {
            _path = path.Select(x =>
            {
                var t = (Vector3)x + Vector3.up * 0.5f + Vector3.right * 0.5f;
                return t;
            }).ToArray();
            _line.positionCount = path.Count;
            _line.SetPositions(_path);
        }
        private void MoveAlongPath(int index)
        {
            Vector3[] newPath = new Vector3[_path.Length - 1];
            for (int i = 0; i < _path.Length - 1; i++)
            {
                newPath[i] = _path[i + 1];
            }
            _path = newPath;
            _line.positionCount = _path.Length;
            _line.SetPositions(_path);
        }
    }
}