using UnityEngine;

namespace _project.scripts.Core.CombatSystem.Projectiles
{
    [CreateAssetMenu(menuName = "ScriptableObject/Combat/Ammunition", fileName = "Ammo")]
    public class AmmunitionSo : ScriptableObject
    {
        public CaliberSo caliberSo;
        public string ammunitionName;
        public float speedModifier = 1f;
        public float damageModifier = 1f;
        public float penetrationModifier = 1f;
        
    }
}