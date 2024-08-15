using System;
using UnityEngine;

namespace _project.scripts.UI
{
    public class GameOverUI : MonoBehaviour
    {
        public static event Action OnRetry;
        public static event Action OnMainMenu;

        public void OnRetryButton()
        {
            OnRetry?.Invoke();
        }
        
        public void OnMainMenuButton()
        {
            OnMainMenu?.Invoke();
        }
        
        public void OnQuitButton()
        {
            Application.Quit();
        }
    }
}