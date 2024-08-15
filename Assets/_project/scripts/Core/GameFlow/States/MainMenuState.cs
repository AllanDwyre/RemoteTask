using System;
using _project.scripts.Network;
using _project.scripts.UI;
using _project.scripts.Utils.StateMachine;
using Unity.Services.Relay;
using UnityEngine;
using WebSocketSharp;

namespace _project.scripts.Core.GameFlow.States
{
    /// <summary>
    /// When we start the game, and we are on the main menu.
    /// </summary>
    public class MainMenuState : IState
    {
        public void Enter()
        {
            GameManager.Instance.Controls.SetUI();
            MainMenu.OnPartyJoin += OnPartyJoin;
            MainMenu.OnPartyCreated += OnPartyCreated;
        }

        public void Exit()
        {
            MainMenu.OnPartyJoin -= OnPartyJoin;
            MainMenu.OnPartyCreated -= OnPartyCreated;
        }

        private async void OnPartyCreated(Action callback)
        {
            string code = "";
            try
            {
                code = await RelayHandler.Singleton.CreateGame();
            }
            catch (Exception e)
            {
                Debug.LogError($"Cannot create game : {e.Message}");
            }
            
            if (code.IsNullOrEmpty())
            {
                Debug.LogError("No code given");
                // TODO : Let's the player know
                return;
            }
            GameManager.Instance.SetPartyCode(code);
            callback();
            GameManager.StateMachine.QueueNextState(new PreparationState());
        }

        private async void OnPartyJoin(string code, Action callback)
        {
            try
            {
                if (code.IsNullOrEmpty())
                    return;

                if (!await RelayHandler.Singleton.JoinGame(code)) return;
                
                callback();
                GameManager.Instance.SetPartyCode(code);
                GameManager.StateMachine.QueueNextState(new PreparationState());
            }
            catch (RelayServiceException relayServiceException)
            {
                if (relayServiceException.Reason == RelayExceptionReason.EntityNotFound)
                {
                    // TODO : Let's the player know
                }
                Debug.LogError(relayServiceException);
            }
        }
        public void Execute()
        {
        }
    }
}