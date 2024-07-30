using System;
using _project.scripts.commands;
using Unity.Netcode;
using UnityEngine;

namespace _project.scripts.Characters
{
        // A character have common system, like health, environnement effect, weapons ...
        // So its here we will handle the logic for all that. Then the child classes can use it
    public abstract class CharacterControllerBase : NetworkBehaviour
    {
        private readonly CommandManager _controller = new();

        private bool HasPermission => IsOwner || IsOwnedByServer;
        public abstract string CharacterName { get; protected set; }

        private bool IsIdle
        {
            get;
            set;
        } = true;
        
        public void HasFinishedCurrentTask()
        {
            if(!HasPermission) return;
            
            IsIdle = true;
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
            if(!HasPermission) return;
            
            if (!EvaluateCommand(command)) return;

            IsIdle = false;
            command.Execute();
        }

        private void ExecuteNextCommand()
        {
            var command = _controller.DequeueCommand();
            if (command == null || !EvaluateCommand(command)) return;
            IsIdle = false;
            command.Execute();
        }

        public void AddCommand(ICommand command)
        {
            if(!HasPermission) return;
            _controller.AddCommand(command);
        }
    }
}