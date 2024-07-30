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
        private int _playerCount;
        private int _playerReadyCount;

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
        public void OnConnectedCountChanged(int idCount)
        {
            _playerCount = idCount;
            numberOfConnectedPlayer.text = $"{_playerCount}/2 players connected";
            for (int i = 0; i < idCount; i++)
            {
                if (_avatars.ContainsKey((ulong)i)) continue;
                
                _avatars.Add(
                    (ulong)i,
                    Instantiate(playersAvatarPrefab, playersAvatarContainer)
                );
            }
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