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
        private UnityTransport _transport;
        
        [SerializeField] private TMP_Text joinCodeText;
        [SerializeField] private TMP_InputField joinInput;
        
        [SerializeField] private GameObject createbutton;
        [SerializeField] private GameObject joinbutton;
        
        [SerializeField] private GameObject joinInputGameObject;
        [SerializeField] private GameObject codeGameObject;
        [SerializeField] private GameObject canvas;
        
        private const int MaxPlayer = 2;
        private bool isReady = false;

        private async void Awake()
        {
            joinInputGameObject.SetActive(false);
            codeGameObject.SetActive(false);
            _transport = FindObjectOfType<UnityTransport>();

            canvas.SetActive(false);
            await Authenticate();
            canvas.SetActive(true);
        }

        
        private async Task Authenticate()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async void CreateGame()
        {
            joinbutton.SetActive(false);
            codeGameObject.SetActive(true);

            Allocation a = await RelayService.Instance.CreateAllocationAsync(MaxPlayer);
            joinCodeText.text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
            
            _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
            
            NetworkManager.Singleton.StartHost();
        }
        
        public async void JoinGame()
        {
            createbutton.SetActive(false);
            joinInputGameObject.SetActive(true);
            
            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinInput.text);
            
            _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
            
            NetworkManager.Singleton.StartClient();
        }

        public void OnReadyClick()
        {
            isReady = !isReady;
            if (!isReady) return;
                
            // TODO : 
            // Ask server for each player connected if they are ready
            // On the other side, we need to activate the ready button after game created or join. 
            // And instantiate a game menu to set up the game's settings and lets the player prepare for it.
            // OR maybe we can put the On ready in another UI because we don't need the relay anymore.
        }
    }
}