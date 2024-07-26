using System;
using System.Collections.Generic;
using _project.scripts.Characters;
using _project.scripts.Input;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace _project.scripts.Core
{
    public class GameManager : NetworkBehaviour
    {
        private const int MaxAgentPerPlayer = 2;
        
        [SerializeField] private InputReader controls;
        [SerializeField] private GameObject agentPrefab;
        [SerializeField] private EGameState stateOnStart;

        public static GameManager Instance;

        public NetworkVariable<EGameState> GameState { get; private set; } =
            new(writePerm: NetworkVariableWritePermission.Owner);

        #region Events

        public event Action<List<AgentController>> OnAgentsInitialized; 
        public event Action<ulong> OnPlayerConnection; 
        

        #endregion
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            DontDestroyOnLoad(this);
        }

        public override void OnNetworkSpawn()
        {
            if(!IsHost) return;
            NetworkManager.Singleton.OnClientConnectedCallback += InitializeClientOnConnection;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
            GameState.Value = stateOnStart;
        }

        private void OnSceneLoaded(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            if (scenename == "Gameplay")
            {
                GameState.Value = EGameState.Gameplay;
                foreach (var id in clientscompleted)
                {
                    InitializeClientOnConnection(id);
                }
            }
            
        }

        public void SetGameState(EGameState state)
        {
            GameState.Value = state;
        }
        
        private void InitializeClientOnConnection(ulong clientId)
        {
            Debug.Log($"{clientId} : is connected");
            
            OnPlayerConnection?.Invoke(clientId);
            
            if( GameState.Value != EGameState.Gameplay) return;

            List<AgentController> agents = new List<AgentController>();
            for (int i = 0; i < MaxAgentPerPlayer; i++)
            {
                // TODO : We need to instantiate the agent in an aware way of the environment and the obstacle (like the agent it-self). And may be look into spawner tiles in map 
                GameObject agent = Instantiate(agentPrefab);
                agent.name = $"agent {i} p:{clientId}";
                agent.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);
                agents.Add(agent.GetComponent<AgentController>());
            }
            OnAgentsInitialized?.Invoke(agents);
        }
       
    }
}