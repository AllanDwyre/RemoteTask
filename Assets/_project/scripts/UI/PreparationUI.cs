using System.Collections.Generic;
using _project.scripts.Core;
using _project.scripts.Network;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _project.scripts.UI
{
    public class PreparationUI : NetworkBehaviour
    {
        // TODO : 
        // Ask server for each player connected if they are ready
        // On the other side, we need to activate the ready button after game created or join. 
        // And instantiate a game menu to set up the game's settings and lets the player prepare for it.
        // OR maybe we can put the On ready in another UI because we don't need the relay anymore.
        [SerializeField] private ReadyState readyControls;

        [SerializeField] private TMP_Text partyCodeText;
        [SerializeField] private Image readyButton;
        [SerializeField] private TMP_Text numberOfConnectedPlayer;
        [SerializeField] private TMP_Text numberOfReadyPlayer;
        [SerializeField] private Transform playersAvatarContainer;
        [SerializeField] private Transform playersAvatarPrefab;

        private readonly Dictionary<ulong, Transform> _avatars = new();
        
        public NetworkVariable<bool> IsReady { get; private set; } = new(writePerm: NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnected;
            readyControls.NbOfReadyPeople.OnValueChanged += OnReadyChanged;
        }

        private void OnReadyChanged(int _, int value)
        {
            int playerTotal = NetworkManager.Singleton.ConnectedClients.Count;
            numberOfReadyPlayer.text = $"{value}/{playerTotal} ready";
            if (value == playerTotal)
            {
                
                NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
            }
        }

        private void OnPlayerDisconnected(ulong id)
        {
            numberOfConnectedPlayer.text = NetworkManager.Singleton.ConnectedClients.Count + "/2 players connected";
            Destroy(_avatars[id].gameObject);
            _avatars.Remove(id);
        }

        private void OnPlayerConnected(ulong id)
        {
            numberOfConnectedPlayer.text = NetworkManager.Singleton.ConnectedClients.Count + "/2 players connected";
            _avatars.Add(
                id,
                Instantiate(playersAvatarPrefab, playersAvatarContainer)
            );
        }

        public void ToggleReadyState()
        {
            IsReady.Value = !IsReady.Value;
            Color ready = new Color(1, 0.56f, 0);
            Color notReady = new Color(0,0.83f, 1);
            
            readyButton.DOColor(IsReady.Value ? ready : notReady ,.3f);
        }
        
        public void Init(string partyCode)
        {
            GetComponent<NetworkObject>().Spawn(true);
            partyCodeText.text = partyCode;
        }
        
        public void OnCopyCode()
        {
            GUIUtility.systemCopyBuffer = partyCodeText.text;
        }

    }
        
}