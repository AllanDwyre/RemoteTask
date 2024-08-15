namespace _project.scripts.Utils.StateMachine
{
    public interface IState
    {
        // public bool IsCompleted { get; }
        public void Enter();
        public void Exit();
        public void Execute();
    }
}