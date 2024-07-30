
using _project.scripts.commands;

namespace _project.scripts.Characters
{
    public class AgentController : CharacterControllerBase
    {
        // handle the connections of different system of the agent : like health, movement, firing. It Can do more that just give the orders. It will have the logic of permissions 
        // example : We have the command "Walking To", we can check the health(or if he is stunt) and determine if he is capable to go there.
        
        // He needs to handle also the information to give, like the name, health, medical property to a giver player. For instance if it's an enemy player, we only need to give non-restrain information 


        public override string CharacterName { get; protected set; } = "Agent";

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