using System;
using UnityEngine;

namespace _project.scripts.Core.AlertSystem
{
    [Serializable]
    public class AlertSettings
    {
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private AlertType type;
        
        public string Title => title;
        public string Description => description;
        public AlertType Type => type;
        
        public AlertSettings(string title, string description, AlertType type)
        {
            this.title = title;
            this.description = description;
            this.type = type;
        }
    }
}