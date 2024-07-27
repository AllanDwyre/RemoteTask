using System.Collections;
using _project.scripts.utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace _project.scripts.Network
{
    public class PreparationNetwork : NetworkBehaviour
    {

        #region Unity Events
        public UnityEvent<ulong> onConnectedPlayer;
        public UnityEvent<ulong> onDisconnectedPlayer;
        [FormerlySerializedAs("onConnectedPlayerChanged")] public UnityEvent<int> onConnectedPlayerCountChanged;
        public UnityEvent<int> onReadyPlayerChanged;
        public UnityEvent<bool> onIsReadyChange;
        public UnityEvent<int> onCountdownStart;
        public UnityEvent onCountdownStop;
        #endregion
        
        public NetworkVariable<int> ReadyCount { get; private set; } = new(writePerm: NetworkVariableWritePermission.Owner);
        public int ConnectedPlayers { get; private set; }

        private bool _isReady = false;
        private Coroutine _countdownCoroutine;
        
        public override void OnNetworkSpawn()
        {
            ReadyCount.OnValueChanged += OnReadyChange;
            if (!IsOwner) return;
            
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnectedChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnectedChanged;
        }

        private void OnReadyChange(int previousValue, int newValue)
        {
            onReadyPlayerChanged?.Invoke(newValue);
            if (ReadyCount.Value == ConnectedPlayers)
            {
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
        
        private void OnConnectedChanged(ulong id)
        {
            UpdatePlayerCountServerRpc(id);
        }
        private void OnDisconnectedChanged(ulong id)
        {
            onDisconnectedPlayer?.Invoke(id);
        }
        [ServerRpc]
        private void UpdatePlayerCountServerRpc(ulong id)
        {
            UpdatePlayerCountClientRpc(NetworkManager.Singleton.ConnectedClients.Count,id);
        }
        
        [ClientRpc]
        private void UpdatePlayerCountClientRpc(int connectedClients, ulong id)
        {
            onConnectedPlayer?.Invoke(id);
            ConnectedPlayers = connectedClients;
            onConnectedPlayerCountChanged?.Invoke(ConnectedPlayers);
        }
        
        private IEnumerator CountDownGameStart()
        {
            for (int i = 0; i < 10; i++)
            {
                onCountdownStart?.Invoke(10 - i);
                yield return Helper.WaitForSeconds(1);
            }
            NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }
    }
}