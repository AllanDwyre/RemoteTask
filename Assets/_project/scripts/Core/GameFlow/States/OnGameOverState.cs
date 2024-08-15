using System;
using _project.scripts.UI;
using _project.scripts.Utils.StateMachine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace _project.scripts.Core.GameFlow.States
{
    /// <summary>
    /// We want to clean the game, serialize some data. set up a retry
    /// </summary>
    public class OnGameOverState : IState
    {
        private bool _isLooser;
        public OnGameOverState(bool isLooser)
        {
            _isLooser = isLooser;
        }

        public bool IsCompleted { get; private set; }
        
        public void Enter()
        {
            GameManager.Instance.Controls.SetUI();
            NetworkManager.Singleton.SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
            
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManagerOnOnLoadComplete;
            // TODO : get the scene ui, and set the good UI for the victorious or looser 
        }

        private void SceneManagerOnOnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (sceneName != "GameOver") return;
            
            GameOverUI.OnRetry += OnRetry;
            GameOverUI.OnMainMenu += OnMainMenu;
        }

        private void OnMainMenu()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
            }
            else
            {
                var id = NetworkManager.Singleton.LocalClientId;
                NetworkManager.Singleton.DisconnectClient(id);
            }
            
            GameManager.StateMachine.QueueNextState(new MainMenuState());
        }

        private void OnRetry()
        {
            GameManager.StateMachine.QueueNextState(new PreparationState());
        }

        public void Exit()
        {
            GameOverUI.OnRetry -= OnRetry;
            GameOverUI.OnMainMenu -= OnMainMenu;
        }
        
        public void Execute()
        {
            // on button event pressed to restart, => preparation state
            // on button event pressed to quit, => main menu state
        }
        
    }
}