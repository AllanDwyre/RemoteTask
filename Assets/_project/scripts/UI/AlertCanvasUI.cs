using System;
using System.Collections.Generic;
using System.Linq;
using _project.scripts.Core.AlertSystem;
using UnityEngine;

namespace _project.scripts.UI
{

    [Serializable]
    public class AlertCustomisation
    {
        [SerializeField] public AlertType alertType;
        [SerializeField] public Color alertColor;
    }
        
    public class AlertCanvasUI : MonoBehaviour
    {
        [SerializeField] private GameObject alertPrefab;
        [SerializeField] private List<AlertCustomisation> alertsTypesDecoration;

        private void Awake()
        {
            AlertController.NewAlert += AddNewAlert;
        }

        public void AddNewAlert(AlertScriptableObject alertInfo, Vector2 position)
        {
            var alert =  Instantiate(alertPrefab, transform);
            if (TryToGetByKey(alertInfo.type, out Color alertColor))
            {
                alert.GetComponent<AlertUI>().Init(alertColor, alertInfo, position);
            }
            else
            {
                Debug.LogError("<color=red>The alert can't be initialized, the type have not a setting </color>");
            }
        }

        private bool TryToGetByKey(AlertType alertType, out Color alertColor)
        {
            bool contains = alertsTypesDecoration.Any(x => x.alertType == alertType);
            alertColor = contains ?  alertsTypesDecoration.First(x => x.alertType == alertType).alertColor : new Color();
            return contains;
        }
    }
}