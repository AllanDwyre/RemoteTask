using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace _project.scripts.Network
{
    public class Relay : MonoBehaviour
    {

        public static Relay Singleton { get; private set; }
        
        [SerializeField] private GameObject canvas;
        
        private UnityTransport _transport;
        private const int MaxPlayer = 2;

        private async void Awake()
        {
            if (Singleton != null)
            {
                Destroy(this);
            }

            Singleton = this;
            _transport = FindObjectOfType<UnityTransport>();
            
            if (_transport == null)
            {
                Debug.LogError("Not Transport Found !");
            }
            
            canvas.SetActive(false);
            await Authenticate();
            canvas.SetActive(true);
        }

        
        private async Task Authenticate()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async Task<string> CreateGame()
        {
            try
            {
                Allocation a = await RelayService.Instance.CreateAllocationAsync(MaxPlayer - 1);
            
                _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
                
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

                return NetworkManager.Singleton.StartHost() ? joinCode : null;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
            
        }
        
        public async Task<bool> JoinGame(string joinCode)
        {
            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
            
            return NetworkManager.Singleton.StartClient();
        }
    }
}