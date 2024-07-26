using System;
using System.Collections.Generic;
using _project.scripts.Input;
using Unity.Netcode;
using UnityEngine;

namespace _project.scripts.Core
{
    public class GameManager : NetworkBehaviour
    {
        private const int MaxAgentPerPlayer = 2;
        
        [SerializeField] private InputReader controls;
        [SerializeField] private GameObject agentPrefab;

        public static GameManager Instance;

        private EGameState _gameState;
        public event Action<EGameState> OnStateChangeEvent;

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
            controls.PauseEvent += HandlePause;
            DontDestroyOnLoad(this);
        }

        public override void OnNetworkSpawn()
        {
            SetGameState(EGameState.Gameplay);
        }

        private void HandlePause()
        {
            // display UI
            SetGameState(EGameState.InUI);
        }

        public void SetGameState(EGameState state)
        {
            _gameState = state;
            NewGameState();
            OnStateChangeEvent?.Invoke(state);
        }

        private void NewGameState()
        {
            if( _gameState != EGameState.Gameplay) return;
            
            IReadOnlyList<ulong> clientsId = NetworkManager.Singleton.ConnectedClientsIds;
            foreach (var id in clientsId)
            {
                // TODO : The current number of the team in static, but I think the preparation phase can allow a dynamic team size, and so will be need to get the data here
                for (int i = 0; i < MaxAgentPerPlayer; i++)
                {
                    // TODO : We need to instantiate the agent in an aware way of the environnement and the obstacle (like the agent it-self). And may be look into spwaner tiles in map 
                    GameObject agent = Instantiate(agentPrefab);
                    agent.name = $"agent {i} p:{id}";
                    agent.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);
                }
            }
        }

    }
}