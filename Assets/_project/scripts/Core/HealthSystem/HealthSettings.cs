using UnityEngine;

namespace _project.scripts.Core.HealthSystem
{
    [CreateAssetMenu(menuName = "ScriptableObject/Character", fileName = "HealthSettings")]
    public class HealthSettings : ScriptableObject
    {
        public int health = 100;
    }
}