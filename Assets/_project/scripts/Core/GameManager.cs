using System;
using System.Collections.Generic;
using _project.scripts.Input;
using Unity.Netcode;
using UnityEngine;

namespace _project.scripts.Core
{
    public class GameManager : NetworkBehaviour
    {
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
                GameObject agent = Instantiate(agentPrefab);
                agent.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);
            }
        }
    }
}