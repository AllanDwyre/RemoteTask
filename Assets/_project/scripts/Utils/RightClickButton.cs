using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _project.scripts.Utils
{
    public class RightClickButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent onRightClick;
 
        [SerializeField] private Color rightClickColor = Color.gray;
 
        [SerializeField] private float rightClickColorDuration = 0.1f;
 
        private Button _button;
 
 
        private void Awake()
        {
            _button = GetComponent<Button>();
        }
 
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                onRightClick?.Invoke();
            }
        }
 
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                StartCoroutine(FadeToRightClickColor());
            }
        }
 
        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                StartCoroutine(FadeToNormalColor());
            }
        }
 
        private IEnumerator FadeToRightClickColor()
        {
            Color originalColor = _button.targetGraphic.color;
            float timeElapsed = 0;
 
            while (timeElapsed < rightClickColorDuration)
            {
                timeElapsed += Time.deltaTime;
                float t = timeElapsed / rightClickColorDuration;
                _button.targetGraphic.color = Color.Lerp(originalColor, rightClickColor, t);
                yield return null;
            }
 
            _button.targetGraphic.color = rightClickColor;
        }
 
        private IEnumerator FadeToNormalColor()
        {
            Color originalColor = _button.targetGraphic.color;
            float timeElapsed = 0;
 
            while (timeElapsed < rightClickColorDuration)
            {
                timeElapsed += Time.deltaTime;
                float t = timeElapsed / rightClickColorDuration;
                _button.targetGraphic.color = Color.Lerp(originalColor, _button.colors.normalColor, t);
                yield return null;
            }
 
            _button.targetGraphic.color = _button.colors.normalColor;
        }
    }
}