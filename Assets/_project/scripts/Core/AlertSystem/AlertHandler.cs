using System;
using UnityEngine;

namespace _project.scripts.Core.AlertSystem
{
    public static class AlertHandler
    {
        public static event Action<AlertSettings, Vector2> NewAlert;

        public static void AddNewAlert(AlertSettings alertInfo, Vector2 position)
        {
            NewAlert?.Invoke(alertInfo, position);
        }
        
    }
}