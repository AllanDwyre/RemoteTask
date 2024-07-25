using _project.scripts.commands;
using UnityEngine;

namespace _project.scripts.Characters
{
        // A character have common system, like health, environnement effect, weapons or abilities(for animals) ...
        // So its here we will handle the logic for all that. Then the child classes can use it
    public abstract class CharacterControllerBase : MonoBehaviour
    {
        private readonly CommandManager _controller = new();
        
        public void HasFinishedCurrentTask() => ExecuteNextCommand();

        protected abstract bool EvaluateCommand(ICommand command);
        
        public void ExecuteCommand(ICommand command)
        {
            if (!EvaluateCommand(command)) return;
           
            command.Execute();
        }
        
        public void ExecuteCommandWithoutEvaluate(ICommand command)
        {
            command.Execute();
        }
        
        public void ExecuteNextCommand()
        {
            var command = _controller.DequeueCommand();
            if (!EvaluateCommand(command)) return;
            command?.Execute();
        }
        
        public void ExecuteNextCommandWithoutEvaluate()
        {
            _controller.DequeueCommand()?.Execute();
        }
        public void AddCommand(ICommand command)
        {
            _controller.AddCommand(command);
        }
    }
}