using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _project.scripts.Input
{
    [CreateAssetMenu(menuName = "InputReader")]
    public class InputReader : ScriptableObject, PlayerActions.IInteractionActions , PlayerActions.IMainActions
    {
        private PlayerActions _controls;

        #region Events

        public event Action<Vector2> MoveEvent;
        public event Action<float> ZoomEvent;
        
        public event Action SelectionEvent; 
        public event Action SelectionCanceledEvent;
        
        public event Action ContextEvent; 
        public event Action QueueActionEvent;
        
        
        public event Action PauseEvent; 
        public event Action ResumeEvent; 

        #endregion
        private void OnEnable()
        {
            if (_controls != null) return;
            
            _controls = new PlayerActions();
            _controls.interaction.SetCallbacks(this);
            _controls.main.SetCallbacks(this);

            SetGameplay(); // for now
        }

        #region SetActionMap
        public void SetUI()
        {
            _controls.Disable();
            // enable menu inputMap
        }
        public void SetGameplay()
        {
            _controls.Disable();
            _controls.interaction.Enable();
            _controls.main.Enable();
        }
        
        public void SetObserverMode()
        {
            _controls.Disable();
            _controls.main.Enable();
            // enable replay inputMap
        }
        #endregion
        public void OnSelection(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                SelectionEvent?.Invoke();
            }
            if (context.phase == InputActionPhase.Canceled)
            {
                SelectionCanceledEvent?.Invoke();
            }
        }

        public void OnContext(InputAction.CallbackContext context)
        {
            if (Keyboard.current.shiftKey.ReadValue() > 0) // Combo aren't well-supported in the new input system
            {
                return;
            }
            ContextEvent?.Invoke();
        }

        public void OnQueueActions(InputAction.CallbackContext context)
        {
            QueueActionEvent?.Invoke();
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                PauseEvent?.Invoke();
                SetUI();
            }
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            ZoomEvent?.Invoke(context.ReadValue<float>());
        }
    }
}