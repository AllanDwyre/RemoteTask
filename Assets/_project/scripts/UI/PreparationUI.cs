using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _project.scripts.UI
{
    public class PreparationUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text partyCodeText;
        [SerializeField] private Image readyButton;
        [SerializeField] private TMP_Text numberOfConnectedPlayer;
        [SerializeField] private TMP_Text numberOfReadyPlayer;
        [SerializeField] private Transform playersAvatarContainer;
        [SerializeField] private Transform playersAvatarPrefab;

        private readonly Dictionary<ulong, Transform> _avatars = new();
        private int _playerCount = 0;
        private int _playerReadyCount = 0;

        public void Init(string partyCode)
        {
            partyCodeText.text = partyCode;
            
        }
        
        public void OnIsReadyChanged(bool isReady)
        {
            Color ready = new Color(1, 0.56f, 0);
            Color notReady = new Color(0,0.83f, 1);
            
            readyButton.DOColor(isReady ? ready : notReady ,.3f);
        }
        public void OnReadyCountChanged(int value)
        {
            _playerReadyCount = value;
            numberOfReadyPlayer.text = $"{_playerReadyCount}/{_playerCount} ready";
        }
        public void OnConnectedCountChanged(int value)
        {
            _playerCount = value;
            numberOfConnectedPlayer.text = $"{_playerCount}/2 players connected";
        }
        
        public void OnPlayerConnected(ulong id)
        {
            _avatars.Add(
                id,
                Instantiate(playersAvatarPrefab, playersAvatarContainer)
            );
        }
        public void OnPlayerDisconnected(ulong id)
        {
            _avatars.Remove(id);
        }

        public void OnCountDown(int second)
        {
            numberOfReadyPlayer.text = $"The game start in {second} seconds...";
        }
        public void OnCountDownStop()
        {
            OnReadyCountChanged(_playerReadyCount);
        }

        public void OnCopyCode()
        {
            GUIUtility.systemCopyBuffer = partyCodeText.text;
        }

    }
        
}