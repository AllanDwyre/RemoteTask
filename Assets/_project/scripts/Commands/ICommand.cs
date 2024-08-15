namespace _project.scripts.commands
{
    public interface ICommand
    {
        public void Execute();
        // public void Unexecute(); // stop or undo the current command
        // public bool OnValidate(); // call the right component to test the validation; 
    }
}