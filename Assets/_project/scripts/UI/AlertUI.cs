using _project.scripts.Core.AlertSystem;
using _project.scripts.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _project.scripts.UI
{
    public class AlertUI : MonoBehaviour
    {
        [SerializeField] private Image _box;
        [SerializeField] private TMP_Text _title;
        // [SerializeField] private TMP_Text _description;

        private Vector2 _location;
        
        public void Init(Color alertColor, AlertSettings alertInfo, Vector2 position)
        {
            _title.text = alertInfo.Title;
            // _description.text = alertInfo.description;
            _box.color = alertColor;
            _location = position;

            // _description.alpha = 0;
            // _description.enabled = false;
            
        }
        
        public void GoToLocation()
        {
            var camPos = Object.FindObjectOfType<PlayerController>().transform.position;
            Object.FindObjectOfType<PlayerController>().transform.position =  new Vector3(_location.x, _location.y, camPos.z);
        }
        
        public void Dismiss()
        {
            Destroy(gameObject);
        }
    }
}