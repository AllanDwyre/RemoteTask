using System;
using System.Collections.Generic;
using _project.scripts.Characters;
using _project.scripts.commands;
using _project.scripts.Core;
using _project.scripts.grid;
using _project.scripts.utils;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _project.scripts.Input
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Input Dependency")]
        [SerializeField] private InputReader controls;

        [Header("CameraSettings")]
        [SerializeField] private float speed = 2f;
        [SerializeField] private float speedMultiplier = 8f;
        [SerializeField] private float minZoom = 15f;
        [SerializeField] private float  maxZoom = 60f;
        [SerializeField] private float zoomMultiplier = 0.01f;
        [SerializeField] private float smoothTime = 0.25f;
        private float _zoom, _velocity;
        
        private TileSelection _tileSelection;
        private List<AgentController> _charactersSelected = new();
        private AgentController _selectedCharacter;
       
        
        private CinemachineVirtualCamera _camera;
        private float _scroll;
        private Vector2 _movement;

        private void Awake()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
            _tileSelection = FindObjectOfType<TileSelection>();
        }

        private void Start()
        {
            GameManager.Instance.OnStateChangeEvent += SetPlayerAgents;
        }

        private void SetPlayerAgents(EGameState state)
        {
            if(state != EGameState.Gameplay || _selectedCharacter != null)
                return;

            _selectedCharacter = FindObjectOfType<AgentController>();
        }

        private void OnEnable()
        {
            controls.QueueActionEvent += QueueAction;
            controls.ContextEvent += GetContextOrAction;
            controls.SelectionCanceledEvent += SelectObject;
            controls.MoveEvent += HandleMovement;
            controls.ZoomEvent += HandleZoom;
        }

        private void Update()
        {
            Zoom();
            Move();
        }

        private void GetContextOrAction()
        {
            Vector2 targetPosition = _tileSelection.WorldHighlightedTilePosition;
            Vector2Int clickedTile = GridUtils.WorldToGrid(targetPosition);

            var command = new WalkingToCommand(clickedTile, _selectedCharacter.GetComponent<CharacterMovement>());
            _selectedCharacter.ExecuteCommand(command);
        }
        
        private void QueueAction()
        {
            Vector2 targetPosition = _tileSelection.WorldHighlightedTilePosition;
            Vector2Int clickedTile = GridUtils.WorldToGrid(targetPosition);
            
            var command = new WalkingToCommand(clickedTile, _selectedCharacter.GetComponent<CharacterMovement>());
            _selectedCharacter.AddCommand(command);
        }

        private void SelectObject()
        {

            Vector2 mousePosition = Helper.Camera.ScreenToWorldPoint(Helper.MousePosition);
            RaycastHit2D hit = Physics2D.CircleCast(mousePosition, .5f, Vector2.zero);

            if (hit.collider != null)
            {
                CharacterControllerBase character = hit.collider.GetComponent<CharacterControllerBase>();

                if (character != null && character is AgentController agent)
                {
                    _selectedCharacter = agent;
                }
            }
        }

        private void HandleZoom(float scroll) => _scroll = scroll;
        private void HandleMovement(Vector2 movement) => _movement = movement;
        private void Zoom()
        {
            _zoom -= _scroll * zoomMultiplier;
            _zoom = Mathf.Clamp(value: _zoom, minZoom, maxZoom);
            _camera.m_Lens.OrthographicSize =
                Mathf.SmoothDamp(_camera.m_Lens.OrthographicSize, _zoom, ref _velocity, smoothTime);
        }

        private void Move()
        {
            float speedMultiplierRatio = (_zoom - minZoom) / maxZoom + 1;
            float multiplier = speedMultiplierRatio * speedMultiplier;
            transform.Translate(_movement * (speed * multiplier * Time.deltaTime));
        }
        
    }
}
