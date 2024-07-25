
using _project.scripts.commands;

namespace _project.scripts.Characters
{
    public class AgentController : CharacterControllerBase
    {
        // handle the connections of different system of the agent : like health, movement, firing. It Can do more that just give the orders. It will have the logic of permissions 
        // example : We have the command "Walking To", we can check the health(or if he is stunt) and determine if he is capable to go there.


        protected override bool EvaluateCommand(ICommand command)
        {
            return true;
        }
    }
}