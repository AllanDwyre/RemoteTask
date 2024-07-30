using System;
using System.Collections.Generic;

namespace _project.scripts.commands
{
    public class CommandManager
    {
        private readonly Queue<ICommand> _commands = new Queue<ICommand>();

        public int Count => _commands.Count;
        
        public void AddCommand(ICommand command)
        {
            _commands.Enqueue(command);
        }
        
        public ICommand DequeueCommand()
        {
            return _commands.TryDequeue(out var command) ? command : null;
        }
        
        
        public ICommand DequeueCommandWithType(Type type)
        {
            if (_commands.TryPeek(out var command))
            {
                if (command.GetType() == type )
                {
                    return DequeueCommand();
                }
            }
            return null;
        }
    }
}