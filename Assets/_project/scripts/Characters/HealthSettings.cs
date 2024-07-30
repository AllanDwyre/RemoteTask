using UnityEngine;

namespace _project.scripts.Characters
{
    [CreateAssetMenu(menuName = "ScriptableObject/Character", fileName = "HealthSettings")]
    public class HealthSettings : ScriptableObject
    {
        public int health = 100;
    }
}