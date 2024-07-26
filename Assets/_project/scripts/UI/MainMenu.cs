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
        [SerializeField] private GameObject preparationMenuCanvas;
        
        [SerializeField] private GameObject titleText;
        
        [SerializeField] private GameObject createButton;
        [SerializeField] private GameObject joinButton;
        
        [SerializeField] private GameObject codesContainer;
        [FormerlySerializedAs("createCodeText")] [SerializeField] private GameObject createCodeButton;
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

        private void ResetMainMenu()
        {
            joinButton.SetActive(true);
            createButton.SetActive(true);
            FadeButton(joinButton,1,.5f);
            FadeButton(createButton, 1, .5f);
            
            joinCodeInput.GetComponent<TMP_InputField>().text = "";
            createCodeButton.GetComponentInChildren<TMP_Text>().text = "";
            
            codesContainer.GetComponent<RectTransform>().DOScaleX(0,0.5f);
            createCodeButton.GetComponent<RectTransform>().DOScaleX(0,0.5f);
            joinCodeInput.GetComponent<RectTransform>().DOScaleX(0,0.5f);
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

        public void OnCopyCode()
        {
            GUIUtility.systemCopyBuffer = createCodeButton.GetComponentInChildren<TMP_Text>().text;
        }
        
        public async void OnCreateClicked()
        {
            if (_hasClickedJoinOrCreateOnce) return;
            _hasClickedJoinOrCreateOnce = true;
  
            createCodeButton.GetComponentInChildren<TMP_Text>().text = await Relay.Singleton.CreateGame();

            GameObject ui = Instantiate(preparationMenuCanvas);
            
            ui.GetComponent<PreparationUI>().Init(createCodeButton.GetComponentInChildren<TMP_Text>().text);
            mainMenuCanvas.SetActive(false);
        }
        
        public void OnJoinClicked()
        {
            if (_hasClickedJoinOrCreateOnce)
            {
                if (!HasSuccessfullyJoined()) return;
                GameObject ui = Instantiate(preparationMenuCanvas);
                ui.GetComponent<PreparationUI>().Init(joinCodeInput.GetComponent<TMP_InputField>().text);
                mainMenuCanvas.SetActive(false);
                return;
            }

            _hasClickedJoinOrCreateOnce = true;
            
            FadeButton(createButton, 0, 1f).onComplete += () => createButton.SetActive(false);
            joinCodeInput.GetComponent<RectTransform>().DOScaleX(1f, 1f);
            codesContainer.GetComponent<RectTransform>().DOScaleX(1f, 1f);
        }

       
        private Tween FadeButton(GameObject button, float endValue, float duration)
        {
            
            button.GetComponent<Image>().DOFade(endValue, duration);
            return button.GetComponentInChildren<TMP_Text>().DOFade(endValue, duration);
        }
        private bool HasSuccessfullyJoined()
        {
            try
            {
                string code = joinCodeInput.GetComponent<TMP_InputField>().text;
                if (code.IsNullOrEmpty())
                {
                    return false;
                }
                Relay.Singleton.JoinGame(code);
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

            return true;
        }
    }
}