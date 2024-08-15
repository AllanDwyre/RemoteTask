using System.Collections.Generic;
using _project.scripts.Characters;
using _project.scripts.Utils.StateMachine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace _project.scripts.Core.GameFlow.States
{
    /// <summary>
    /// We want to initialize the gameplay state, by spawning entities, assigning them to players and set an objective on the map
    /// It also means to generate a map
    /// </summary>
    public class GameplayInitializeState : IState
    {
        private readonly List<Agent> _agents = new List<Agent>();

        public bool IsCompleted { get; private set; }
        private readonly bool _isForcedLoad;

        public GameplayInitializeState(bool isForcedLoad = false)
        {
            _isForcedLoad = isForcedLoad;
        }
        
        public void Enter()
        {
#if UNITY_EDITOR
            if (_isForcedLoad)
            {
                var currentId = NetworkManager.Singleton.LocalClientId;
                InitializeClientOnConnection(currentId);
                return;
            }
#endif
            NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManagerOnOnLoadComplete;
        }

        private void SceneManagerOnOnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (sceneName != "Gameplay") return;
            InitializeClientOnConnection(clientId);
            GameManager.StateMachine.QueueNextState(new GameplayState(_agents));
        }


        public void Exit()
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= SceneManagerOnOnLoadComplete;
        }
        
        public void Execute()
        {
        }
        
        private void InitializeClientOnConnection(ulong clientId)
        {
            for (int i = 0; i < GameManager.MaxAgent; i++)
            {
                _agents.Add(GameManager.Instance.InstantiateAgents(clientId, i));
            }
        }
    }
}