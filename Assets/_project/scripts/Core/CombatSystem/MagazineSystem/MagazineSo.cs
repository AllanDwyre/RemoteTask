using UnityEngine;

namespace _project.scripts.Core.CombatSystem.MagazineSystem
{
    [CreateAssetMenu(menuName = "ScriptableObject/Combat/Magazine",fileName = "magazine")]
    public class MagazineSo : ScriptableObject
    {
        public CaliberSo caliberRequired;
        public int capacity;
    }
}