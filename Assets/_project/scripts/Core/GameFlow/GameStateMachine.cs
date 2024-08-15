using _project.scripts.Input;
using _project.scripts.Utils.StateMachine;

namespace _project.scripts.Core.GameFlow
{
    public class GameStateMachine  : StateMachine
    {
        public GameStateMachine(IState currentState, InputReader controls, IState idleState = null) : base(currentState, idleState)
        {
        }
    }
}