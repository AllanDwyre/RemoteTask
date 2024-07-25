using _project.scripts.Characters;
using UnityEngine;

namespace _project.scripts.commands
{
    public class WalkingToCommand : ICommand
    {
        private Vector2Int _toPosition;
        private CharacterMovement _agentReceiver;

        public WalkingToCommand(Vector2Int toPosition, CharacterMovement agentReceiver)
        {
            _toPosition = toPosition;
            _agentReceiver = agentReceiver;
        }

        public void Execute()
        {
            Debug.Log("Executed");
            _agentReceiver.MoveToTarget(_toPosition);
        }
    }
}