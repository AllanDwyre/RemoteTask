using System;
using UnityEngine;

namespace _project.scripts.Core.AlertSystem
{
    public static class AlertController
    {
        public static event Action<AlertScriptableObject, Vector2> NewAlert;

        public static void AddNewAlert(AlertScriptableObject alertInfo, Vector2 position)
        {
            NewAlert?.Invoke(alertInfo, position);
        }
    }
}