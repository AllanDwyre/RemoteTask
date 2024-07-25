using System.Collections;
using System.Collections.Generic;
using _project.scripts.commands;
using _project.scripts.grid;
using _project.scripts.pathfinding;
using UnityEngine;

namespace _project.scripts.Characters
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private Vector2 gridSize = Vector2.one;
        [SerializeField] private ObstacleTileMap obstacleTileMap;
        

        private Vector2 _targetPosition;
        private bool _isMoving = false;
        private MovementDirection _currentDirection = MovementDirection.North;
        private Coroutine _currentCoroutine;
        private Queue<ICommand> _commands = new Queue<ICommand>();
        private void Update()
        {
            if (_isMoving)
            {
                MoveTowardsTarget();
            }
            else
            {
                if (_commands.TryDequeue(out var command))
                {
                    command.Execute();
                }
            }
        }

        public void AddCommand(ICommand command)
        {
            _commands.Enqueue(command);
        }
        public void ExecuteCommand(ICommand command)
        {
            _commands.Clear();
            command.Execute();
        }
        public void MoveToTarget(Vector2Int targetPosition)
        {
            _targetPosition = targetPosition;
            if (_isMoving && _currentCoroutine != null)
            {
                _isMoving = false;
                StopCoroutine(_currentCoroutine);
            }
            
            if (!obstacleTileMap.IsTileObstacle(_targetPosition) && _targetPosition != Vector2Int.zero)
            {
                FindPathToTargetPosition();
            }
        }
        
        private void FindPathToTargetPosition()
        {
            Vector2 startPosition = transform.position;
            List<Vector2> path = AStar.FindPath(startPosition, _targetPosition, gridSize, obstacleTileMap.IsTileObstacle);

            if (path != null && path.Count > 0)
            {
                _currentCoroutine = StartCoroutine(MoveAlongPath(path));
            }
        }
        
        private IEnumerator MoveAlongPath(List<Vector2> path)
        {
            _isMoving = true;
            int currentWaypointIndex = 0;

            while (currentWaypointIndex < path.Count)
            {
                _targetPosition = path[currentWaypointIndex] + gridSize / 2;

                while ((Vector2) transform.position != _targetPosition)
                {
                    float step = moveSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);

                    yield return new WaitForFixedUpdate();
                }

                currentWaypointIndex++;
            }
            _isMoving = false;
        }
        private void MoveTowardsTarget()
        {
            Vector2 direction = (_targetPosition - (Vector2)transform.position).normalized;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // TODO : -get the corresponding MovementDirection
            }
        }
    }
}
