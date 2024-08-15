using System;
using _project.scripts.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace _project.scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuCanvas;
        
        [SerializeField] private GameObject titleText;
        
        [SerializeField] private GameObject createButton;
        [SerializeField] private GameObject joinButton;
        
        [SerializeField] private GameObject codesContainer;
        [SerializeField] private GameObject joinCodeInput;

        private bool _hasClickedJoinOrCreateOnce;
        public static event Action<string,Action> OnPartyJoin;
        public static event Action<Action> OnPartyCreated;
        public static event Action OnNetworkInit;
        private void Start()
        {
            DOTween.Init();

            titleText.transform.DOScale(2f, 2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
        }

        private void OnDestroy()
        {
            DOTween.Kill(titleText.transform);
        }   

        private void OnEnable()
        {
            _hasClickedJoinOrCreateOnce = false;
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
        public void OnCreateClicked()
        {
            if (_hasClickedJoinOrCreateOnce) return;
            _hasClickedJoinOrCreateOnce = true;
            OnPartyCreated?.Invoke(OnSuccess);
        }
        public void OnJoinClicked()
        {
            if (JoinUIAnimation()) return;
            string code = joinCodeInput.GetComponent<TMP_InputField>().text;
            OnPartyJoin?.Invoke(code, OnSuccess);
        }

        private void OnSuccess()
        {
            StartCoroutine(NetworkUtils.WaitForNetworkReady(() => OnNetworkInit?.Invoke()));
        }

        #endregion
        #region Utils
        private void ResetMainMenu()
        {
            joinButton.SetActive(true);
            createButton.SetActive(true);
            joinCodeInput.SetActive(false);
            
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
            joinCodeInput.SetActive(true);
            joinCodeInput.GetComponent<RectTransform>().DOScaleX(1f, .25f);
            codesContainer.GetComponent<RectTransform>().DOScaleX(1f, .25f);
            _hasClickedJoinOrCreateOnce = true;
            return true;

        }
        private static void FadeButton(GameObject button, float endValue, float duration)
        {
            button.GetComponent<Image>().DOFade(endValue, duration);
            button.GetComponentInChildren<TMP_Text>().DOFade(endValue, duration);
        }
        #endregion
    }
}
