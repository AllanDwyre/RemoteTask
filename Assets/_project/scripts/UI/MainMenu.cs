using System.Threading.Tasks;
using _project.scripts.utils;
using DG.Tweening;
using TMPro;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using WebSocketSharp;
using Relay = _project.scripts.Network.Relay;

namespace _project.scripts.UI
{
    public class MainMenu : MonoBehaviour
    {

        [SerializeField] private GameObject mainMenuCanvas;
        [SerializeField] private Canvas preparationMenuCanvas;
        
        [SerializeField] private GameObject titleText;
        
        [SerializeField] private GameObject createButton;
        [SerializeField] private GameObject joinButton;
        
        [SerializeField] private GameObject codesContainer;
        [SerializeField] private GameObject joinCodeInput;

        private bool _hasClickedJoinOrCreateOnce;

        private void Start()
        {
            DOTween.Init();

            titleText.transform.DOScale(2f, 2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
        }

        private void OnEnable()
        {
            _hasClickedJoinOrCreateOnce = false;
            ResetMainMenu();
        }
        
        public void OnBackClicked()
        {
            if (!_hasClickedJoinOrCreateOnce)
            {
                Application.Quit();
            }
            
            _hasClickedJoinOrCreateOnce = false;
            ResetMainMenu();
        }

        

        #region NetworkManagement
        public async void OnCreateClicked()
        {
            if (_hasClickedJoinOrCreateOnce) return;
            _hasClickedJoinOrCreateOnce = true;
  
            string code = await Relay.Singleton.CreateGame();
            if (code == null)
            {
                Debug.LogError("Server didn't start properly.");
                _hasClickedJoinOrCreateOnce = false;
            }
            StartCoroutine(NetworkUtils.WaitForNetworkReady(() => PreparationUI(code)));
        }


        public async void OnJoinClicked()
        {
            if (JoinUIAnimation()) return;
            
            
            string code = joinCodeInput.GetComponent<TMP_InputField>().text;
            if (! await HasSuccessfullyJoined(code)) return;
            StartCoroutine(NetworkUtils.WaitForNetworkReady(() => PreparationUI(code)));
        }
        private void PreparationUI(string code)
        {
            preparationMenuCanvas.enabled = true;
            preparationMenuCanvas.GetComponent<PreparationUI>().Init(code);
            mainMenuCanvas.SetActive(false);
        }
        #endregion
        #region Utils
        private void ResetMainMenu()
        {
            joinButton.SetActive(true);
            createButton.SetActive(true);
            FadeButton(joinButton,1,.25f);
            FadeButton(createButton, 1, .25f);
            
            joinCodeInput.GetComponent<TMP_InputField>().text = "";
            
            codesContainer.GetComponent<RectTransform>().DOScaleX(0,0.5f);
            joinCodeInput.GetComponent<RectTransform>().DOScaleX(0,0.5f);
        }
        private bool JoinUIAnimation()
        {
            if (_hasClickedJoinOrCreateOnce) return false;
            
            createButton.SetActive(false);
            joinCodeInput.GetComponent<RectTransform>().DOScaleX(1f, .25f);
            codesContainer.GetComponent<RectTransform>().DOScaleX(1f, .25f);
            _hasClickedJoinOrCreateOnce = true;
            return true;

        }


        private Tween FadeButton(GameObject button, float endValue, float duration)
        {
            
            button.GetComponent<Image>().DOFade(endValue, duration);
            return button.GetComponentInChildren<TMP_Text>().DOFade(endValue, duration);
        }
        
        private async Task<bool> HasSuccessfullyJoined(string code )
        {
            try
            {
                if (code.IsNullOrEmpty())
                {
                    return false;
                }
                return await Relay.Singleton.JoinGame(code);
            }
            catch (RelayServiceException relayServiceException)
            {
                if (relayServiceException.Reason == RelayExceptionReason.EntityNotFound)
                {
                    // TODO : Let's the player know
                    
                }
                Debug.LogError(relayServiceException);
                return false;
            }
        }
        #endregion
    }
}
