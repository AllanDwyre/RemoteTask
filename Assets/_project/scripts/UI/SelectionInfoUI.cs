using _project.scripts.Characters;
using _project.scripts.Core.HealthSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _project.scripts.UI
{
    public class SelectionInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text currentTaskText;
        [SerializeField] private TMP_Text queueTasksText;
        [SerializeField] private Image healthBarImage;

        private void Start()
        {
            GetComponent<Canvas>().enabled = false;
        }

        public void OnSelection(GameObject selection)
        {
            GetComponent<Canvas>().enabled = true;
            if (selection.TryGetComponent<CharacterBase>(out var character))
            {
                titleText.text = character.CharacterName;
                HandleHealthUI(character.GetComponent<HealthComponent>());

                currentTaskText.text = "Don't know yet";
                currentTaskText.text = "Don't know yet";
                return;
            }
            
            if (selection.TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
            {
                HandleHealthUI(healthComponent);
                queueTasksText.text = "";
                currentTaskText.text = "";
            }
        }

        private void HandleHealthUI(HealthComponent healthComp)
        {
            
            float healthValue = Mathf.Clamp((float)healthComp.Health.Value / healthComp.MaxHealth, 0f, 1f);
            healthBarImage.rectTransform.DOScaleX(healthValue, 0.2f);
        }

        public void OnDeselect() => GetComponent<Canvas>().enabled = false;
    }
}