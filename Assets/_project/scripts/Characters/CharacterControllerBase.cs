using System;
using _project.scripts.commands;
using UnityEngine;

namespace _project.scripts.Characters
{
        // A character have common system, like health, environnement effect, weapons or abilities(for animals) ...
        // So its here we will handle the logic for all that. Then the child classes can use it
    public abstract class CharacterControllerBase : MonoBehaviour
    {
        private readonly CommandManager _controller = new();

        protected bool IsIdle
        {
            get;
            private set;
        } = false;
        
        public void HasFinishedCurrentTask()
        {
            ExecuteNextCommand();
        }

        protected abstract bool EvaluateCommand(ICommand command);

        protected void Update()
        {
            if(!IsIdle || _controller.Count == 0) return;
            ExecuteNextCommand();
        }

        public void ExecuteCommand(ICommand command)
        {
            if (!EvaluateCommand(command)) return;

            IsIdle = true;
            command.Execute();
        }
        
        public void ExecuteCommandWithoutEvaluate(ICommand command)
        {
            IsIdle = true;
            command.Execute();
        }
        
        public void ExecuteNextCommand()
        {
            var command = _controller.DequeueCommand();
            if (command == null || !EvaluateCommand(command)) return;
            IsIdle = true;
            command.Execute();
        }
        
        public void ExecuteNextCommandWithoutEvaluate()
        {
            var command = _controller.DequeueCommand();
            if (command == null) return;
            
            IsIdle = true;
            command.Execute();
        }
        public void AddCommand(ICommand command)
        {
            _controller.AddCommand(command);
        }
    }
}