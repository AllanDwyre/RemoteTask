using System;
using System.Collections;
using System.Collections.Generic;
using _project.scripts.grid;
using _project.scripts.pathfinding;
using _project.scripts.Utils;
using Unity.Netcode;
using UnityEngine;

namespace _project.scripts.Characters
{
    public class MovementComponent : NetworkBehaviour
    {
        [Header("Movement Settings")]
        
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private Vector2 gridSize = Vector2.one;
        
        public Vector2 TargetPosition { get; private set; }
        
        private bool _isMoving;
        private bool _hadCommand;
        private ObstacleTileMap _obstacleTileMap;
        private Vector2 _moveToPosition;
        
        // private MovementDirection _currentDirection = MovementDirection.North;
        private Coroutine _currentCoroutine;
        private CharacterBase _base;
        private bool _isDisable;

        public event Action<List<Vector2>> OnNewPath;
        public event Action<int> OnMoveAlongPath;
        
        private void Awake()
        {
            _obstacleTileMap = FindObjectOfType<ObstacleTileMap>();
            _base = GetComponent<CharacterBase>();
        }
        public override void OnNetworkSpawn()
        {
            if(!IsOwner) Destroy(this);
        }
        private void Update()
        {
            if (_isDisable) return;
            
           
            HandleDirection();
            
            MoveCompleted();
        }

        

        #region Public
        public void EnableMovement()
        {
            _isDisable = true;
        }
        public void DisableMovement()
        {
            _isDisable = true;
        }
        
        public void MoveToTarget(Vector2Int targetPosition)
        {
            if (_isDisable) return;
            
            _hadCommand = true;
            
            if (_obstacleTileMap.IsTileObstacle(targetPosition) ||  (Vector2) targetPosition == Vector2Int.zero)
            {
                return;
            }
            
            TargetPosition = targetPosition;
            
            if (_isMoving && _currentCoroutine != null)
            {
                _isMoving = false;
                StopCoroutine(_currentCoroutine);
            }
     
            FindPath();
        }

        

        #endregion
        
        private void FindPath()
        {
            Vector2 startPosition = transform.position;
            List<Vector2> path = AStar.FindPath(startPosition, TargetPosition, gridSize, _obstacleTileMap.IsTileObstacle);

            if (path is { Count: > 0 })
            {
                OnNewPath?.Invoke(path);
                _currentCoroutine = StartCoroutine(MoveAlongPath(path));
            }
        }
        private IEnumerator MoveAlongPath(List<Vector2> path)
        {
            _isMoving = true;
            int currentWaypointIndex = 0;

            while (currentWaypointIndex < path.Count)
            {
                _moveToPosition = path[currentWaypointIndex] + gridSize / 2;

                while ((Vector2) transform.position != _moveToPosition)
                {
                    float step = moveSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, _moveToPosition, step);

                    yield return Helper.WaitForFixedUpdate;
                }

                OnMoveAlongPath?.Invoke(currentWaypointIndex);
                currentWaypointIndex++;
            }
            _isMoving = false;
        }
        
        private void MoveCompleted()
        {
            if (_isMoving || !_hadCommand) return;
            
            _hadCommand = false;
            _base.OnTaskCompleted();
        }
        private void HandleDirection()
        {
            if (!_isMoving) return;
            
            Vector2 direction = (_moveToPosition - (Vector2)transform.position).normalized;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // TODO : -get the corresponding MovementDirection
            }
        }

        
    }
}
