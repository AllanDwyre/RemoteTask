using _project.scripts.Input;
using _project.scripts.Utils.StateMachine;
using Unity.Netcode;

namespace _project.scripts.Core.GameFlow
{
    public class GameStateMachine  : StateMachine, INetworkSerializable

    {
        public GameStateMachine(IState currentState, InputReader controls, IState idleState = null) : base(currentState, idleState)
        {
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
        }
    }
}