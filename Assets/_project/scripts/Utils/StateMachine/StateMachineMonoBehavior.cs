using UnityEngine;

namespace _project.scripts.Utils.StateMachine
{
    /// <summary>
    /// Reference all state, oversee the active state, handle transition between states, calls methods of states
    /// </summary>
    public abstract class StateMachineMonoBehavior : MonoBehaviour
    {
        private IState _currentState;
        private readonly IState _idleState;
        private IState _queuedState;

        public IState CurrentState => _currentState;

        protected StateMachineMonoBehavior(IState currentState, IState idleState = null)
        {
            _idleState = idleState ?? currentState;
            _currentState = currentState;
            _currentState.Enter();
        }

        public void QueueNextState(IState nextState) => _queuedState = nextState;

        protected virtual void Update()
        {
            if (_queuedState != null)
            {
                TransitionToQueuedState();
            }

            (_currentState ?? _idleState)?.Execute();
        }

        private void TransitionToQueuedState()
        {
            _currentState.Exit();
            _currentState = _queuedState;
            _queuedState = null;
            _currentState.Enter();
        }
    }
}