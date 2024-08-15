using _project.scripts.Core.CombatSystem.Projectiles;
using UnityEngine;

namespace _project.scripts.Core.CombatSystem
{
    [CreateAssetMenu(menuName = "ScriptableObject/Combat/Caliber", fileName = "caliber")]
    public class CaliberSo : ProjectileSetting
    {
        public string caliberName;
        public float bulletSpeed;
    }
}