using _project.scripts.Characters;
using UnityEngine;

namespace _project.scripts.commands
{
    public class WalkingToCommand : ICommand
    {
        public Vector2Int ToPosition { get; private set;}
        public MovementComponent Receiver { get; private set;}

        public WalkingToCommand(Vector2Int toPosition, MovementComponent receiver)
        {
            ToPosition = toPosition;
            Receiver = receiver;
        }

        public void Execute()
        {
            Receiver.MoveToTarget(ToPosition);
        }
    }
}