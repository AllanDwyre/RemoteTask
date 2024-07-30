using UnityEngine;

namespace _project.scripts.Core.AlertSystem
{
    [CreateAssetMenu(menuName = "ScriptableObject/Core/Alert", fileName = "AlertInfo")]
    public class AlertScriptableObject : ScriptableObject
    {
        public string title;
        public string description;
        public AlertType type;
    }
}