using System.Collections.Generic;
using System.Linq;
using _project.scripts.Characters;
using _project.scripts.Core.HealthSystem;
using _project.scripts.Utils.StateMachine;

namespace _project.scripts.Core.GameFlow.States
{
    /// <summary>
    /// All the generation of the game is done, the player have it all, the loading screen is turn off, the game start
    /// </summary>
    public class GameplayState : IState
    {
        private readonly List<Agent> _allAgents;
        public GameplayState(List<Agent> allAgents)
        {
            _allAgents = allAgents;
        }

        public void Enter()
        {
            // TODO : Event on objective complete
            GameManager.Instance.Controls.SetGameplay();
            Agent.OnAgentLoose += OnAgentLoose;
        }

        public void Exit()
        {
            Agent.OnAgentLoose -= OnAgentLoose;
        }

        public void Execute()
        {
            // will be responsible to moderate the difficulty
        }
        
        private void OnAgentLoose(ulong clientId)
        {
            bool hasRemainingAgent = _allAgents.Where(x => x.OwnerClientId == clientId)
                .Any(a => a.HealthComp.HealthStatus is EHealthStatus.Alive);
            
            if (hasRemainingAgent) return;
            
            GameManager.Instance.PlayerEndOfGame(clientId);
        }
    }
}