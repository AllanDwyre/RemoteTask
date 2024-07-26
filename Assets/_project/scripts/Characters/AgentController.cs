
using _project.scripts.commands;

namespace _project.scripts.Characters
{
    public class AgentController : CharacterControllerBase
    {
        // handle the connections of different system of the agent : like health, movement, firing. It Can do more that just give the orders. It will have the logic of permissions 
        // example : We have the command "Walking To", we can check the health(or if he is stunt) and determine if he is capable to go there.


        protected override bool EvaluateCommand(ICommand command)
        {
            if (!EvaluateWalkToCommand(command)) return false;
            
            return true;
        }

        private static bool EvaluateWalkToCommand(ICommand command)
        {
            if (command is not WalkingToCommand walkingToCommand) return true;
            
            if (walkingToCommand.ToPosition == walkingToCommand.Receiver.TargetPosition) return false;

            return true;
        }
    }
}