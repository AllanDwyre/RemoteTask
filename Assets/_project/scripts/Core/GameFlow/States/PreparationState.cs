using _project.scripts.Network;
using _project.scripts.Utils.StateMachine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace _project.scripts.Core.GameFlow.States
{
    /// <summary>
    /// We want to gather all the data needed to launch the game.
    /// The game settings, the players id & names, their agents apparent, and load outs.
    /// </summary>
    public class PreparationState : IState
    {
        public void Enter()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Preparation", LoadSceneMode.Single);
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
            PreparationNetwork.OnGameStarted += OnGameStarted;
        }

        private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (sceneName != "Preparation") return;
            GameManager.Instance.InitPreparationUI();
        }

        private void OnGameStarted()
        {
            GameManager.StateMachine.QueueNextState(new GameplayInitializeState());
        }

        public void Exit()
        {
            PreparationNetwork.OnGameStarted -= OnGameStarted;
        }

        public void Execute()
        {
        }
    }
}