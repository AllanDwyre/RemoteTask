using System.Collections;
using System.Collections.Generic;
using _project.scripts.commands;
using _project.scripts.grid;
using _project.scripts.pathfinding;
using Unity.Netcode;
using UnityEngine;

namespace _project.scripts.Characters
{
    public class CharacterMovement : NetworkBehaviour
    {
        [Header("Movement Settings")]
        
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private Vector2 gridSize = Vector2.one;
        [SerializeField] private ObstacleTileMap obstacleTileMap;
        

        private Vector2 _targetPosition;
        private bool _isMoving = false;
        private bool _hadCommand = false;
        private MovementDirection _currentDirection = MovementDirection.North;
        private Coroutine _currentCoroutine;
        private CharacterControllerBase _controllerBase;

        private void Awake()
        {
            obstacleTileMap = FindObjectOfType<ObstacleTileMap>();
            _controllerBase = GetComponent<CharacterControllerBase>();
        }

        public override void OnNetworkSpawn()
        {
            if(!IsOwner) Destroy(this);
        }

        private void Update()
        {
            if (_isMoving)
            {
                MoveTowardsTarget();
            }
            else if (_hadCommand)
            {
                _hadCommand = false;
                _controllerBase.HasFinishedCurrentTask();
            }
        }
        
        public void MoveToTarget(Vector2Int targetPosition)
        {
            _hadCommand = true;
            
            if (obstacleTileMap.IsTileObstacle(targetPosition) ||  (Vector2) targetPosition == Vector2Int.zero)
            {
                return;
            }
            
            _targetPosition = targetPosition;
            
            if (_isMoving && _currentCoroutine != null)
            {
                _isMoving = false;
                StopCoroutine(_currentCoroutine);
            }
     
            FindPathToTargetPosition();
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
