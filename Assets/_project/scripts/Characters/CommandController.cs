using System.Collections.Generic;
using _project.scripts.commands;
using UnityEngine;

namespace _project.scripts.Characters
{
    public class CommandController : MonoBehaviour
    {
        private Queue<ICommand> _commands;

        public void AddCommand(ICommand command)
        {
            _commands.Enqueue(command);
        }
    }
}