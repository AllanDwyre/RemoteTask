using System;
using System.Collections;
using _project.scripts.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace _project.scripts.Network
{
    public class PreparationNetwork : NetworkBehaviour
    {
        [SerializeField] private int secondsAfterReady = 10;
        #region Unity Events
        public UnityEvent<ulong> onDisconnectedPlayer;
        public UnityEvent<int> onConnectedPlayerCountChanged;
        public UnityEvent<int> onReadyPlayerChanged;
        public UnityEvent<bool> onIsReadyChange;
        public UnityEvent<int> onCountdownStart;
        public UnityEvent onCountdownStop;
        #endregion
        
        public NetworkVariable<int> ReadyCount { get; private set; } = new(writePerm: NetworkVariableWritePermission.Owner);
        public int ConnectedPlayers { get; private set; }

        private bool _isReady;
        private Coroutine _countdownCoroutine;

        public static event Action OnGameStarted;
        public override void OnNetworkSpawn()
        {
            ReadyCount.OnValueChanged += OnReadyChange;
            if (!IsOwner) return;
            
            UpdatePlayerCountServerRpc();
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnectedChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectedChanged;
        }

        private void OnReadyChange(int previousValue, int newValue)
        {
            onReadyPlayerChanged?.Invoke(newValue);
            if (ReadyCount.Value == ConnectedPlayers)
            {
                secondsAfterReady = ConnectedPlayers == 1 ? 3 : secondsAfterReady;
                _countdownCoroutine = StartCoroutine(CountDownGameStart());
            }
            else if (previousValue >= ConnectedPlayers && newValue < ConnectedPlayers)
            {
                StopCoroutine(_countdownCoroutine);
                onCountdownStop?.Invoke();
            }
        }

        public void OnReadyClicked()
        {
            _isReady = !_isReady;
            onIsReadyChange?.Invoke(_isReady);
            UpdateIsReadyServerRpc(_isReady);

        }
        
        [ServerRpc(RequireOwnership = false)]
        private void UpdateIsReadyServerRpc(bool isReady)
        {
            int count = ReadyCount.Value + (isReady ? 1 : -1);
            ReadyCount.Value = Mathf.Max(0, count);
        }
        
        private void OnDisconnectedChanged(ulong id)
        {
            onDisconnectedPlayer?.Invoke(id);
        }
        private void OnConnectedChanged(ulong id)
        {
            UpdatePlayerCountServerRpc();
        }
        [ServerRpc]
        private void UpdatePlayerCountServerRpc()
        {
            UpdatePlayerCountClientRpc(NetworkManager.Singleton.ConnectedClients.Count);
        }
        
        [ClientRpc]
        private void UpdatePlayerCountClientRpc(int connectedClients)
        {
            ConnectedPlayers = connectedClients;
            onConnectedPlayerCountChanged?.Invoke(ConnectedPlayers);
        }
        
        private IEnumerator CountDownGameStart()
        {
            for (int i = 0; i < secondsAfterReady; i++)
            {
                onCountdownStart?.Invoke(secondsAfterReady - i);
                yield return Helper.WaitForSeconds(1);
            }
            OnGameStarted?.Invoke();
        }
    }
}