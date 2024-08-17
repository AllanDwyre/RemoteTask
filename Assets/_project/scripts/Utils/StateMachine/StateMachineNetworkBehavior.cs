using Unity.Netcode;
using UnityEngine;

namespace _project.scripts.Utils.StateMachine
{
    /// <summary>
    /// Reference all state, oversee the active state, handle transition between states, calls methods of states
    /// </summary>
    public abstract class StateMachineNetworkBehavior : NetworkBehaviour
    {
        protected IState CurrentState;
        private readonly IState _idleState;
        private IState _queuedState;


        public override void OnNetworkSpawn()
        {
            CurrentState.Enter();
        }

        public void QueueNextState(IState nextState) => _queuedState = nextState;

        protected virtual void Update()
        {
            if (!IsServer) return;
            
            if (_queuedState != null)
            {
                TransitionToQueuedStateServerRpc();
            }

            (CurrentState ?? _idleState)?.Execute();
        }
        
        [ServerRpc]
        private void TransitionToQueuedStateServerRpc()
        {
            TransitionToQueuedStateClientRpc();
        }
        
        [ClientRpc]
        private void TransitionToQueuedStateClientRpc()
        {
            CurrentState.Exit();
            CurrentState = _queuedState;
            _queuedState = null;
            CurrentState.Enter();
        }
    }
}