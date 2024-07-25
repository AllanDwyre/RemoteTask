using System.Collections.Generic;
using _project.scripts.Characters;
using _project.scripts.commands;
using _project.scripts.grid;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace _project.scripts.Input
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class PlayerController : MonoBehaviour
    {
        [Header("CameraSettings")]
        [SerializeField] private float speed = 2f;
        [SerializeField] private float speedMultiplier = 8f;
        [SerializeField] private float minZoom = 15f;
        [SerializeField] private float  maxZoom = 60f;
        [SerializeField] private float zoomMultiplier = 0.01f;
        [SerializeField] private float smoothTime = 0.25f;
        private float _zoom, _velocity;
        
        [Header("World Controls Dependency")]
        [SerializeField] private TileSelection tileSelection;
        [SerializeField] private CharacterMovement characterSelected;
       
        
        private PlayerActions _controls;
        private CinemachineVirtualCamera _camera;

        private void Awake()
        {
            _controls = new PlayerActions();
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void OnEnable()
        {
            _controls.Enable();
            _controls.interaction.Context.canceled += _ => GetContextOrAction();
            _controls.interaction.QueueActions.performed += QueueAction;
            _controls.interaction.Selection.canceled += _ => SelectObject();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        private void Update()
        {
            Zoom();
            Move();
        }

        private void GetContextOrAction()
        {
            if (Keyboard.current.shiftKey.ReadValue() > 0) // Combo aren't well-supported in the new input system
            {
                return;
            }
            Debug.Log("Clicked !");
            Vector2 targetPosition = tileSelection.GetWorldHighlightedTilePosition;
            Vector2Int clickedTile = GridUtils.WorldToGrid(targetPosition);

            var command = new WalkingToCommand(clickedTile, characterSelected);
            characterSelected.ExecuteCommand(command);
        }
        
        private void QueueAction(InputAction.CallbackContext context)
        {
            Debug.Log("Clicked Queue!");
            Vector2 targetPosition = tileSelection.GetWorldHighlightedTilePosition;
            Vector2Int clickedTile = GridUtils.WorldToGrid(targetPosition);
            
            var command = new WalkingToCommand(clickedTile, characterSelected);
            characterSelected.AddCommand(command);
        }

        private void SelectObject()
        {
        }

        private void Zoom()
        {
            float scroll = _controls.main.Zoom.ReadValue<float>();
            _zoom -= scroll * zoomMultiplier;
            _zoom = Mathf.Clamp(value: _zoom, minZoom, maxZoom);
            _camera.m_Lens.OrthographicSize =
                Mathf.SmoothDamp(_camera.m_Lens.OrthographicSize, _zoom, ref _velocity, smoothTime);
        }

        private void Move()
        {
            Vector2 movement = _controls.main.Movement.ReadValue<Vector2>();
            float speedMultiplierRatio = (_zoom - minZoom) / maxZoom + 1;
            float multiplier = speedMultiplierRatio * speedMultiplier;
            transform.Translate(movement * (speed * multiplier * Time.deltaTime));
        }
    }
}
