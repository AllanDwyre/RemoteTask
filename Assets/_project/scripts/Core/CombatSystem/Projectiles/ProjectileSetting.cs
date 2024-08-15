using UnityEngine;

// https://zero-sievert.fandom.com/wiki/Ammunition
// https://escapefromtarkov.fandom.com/wiki/Ballistics#7.62x25mm_Tokarev_anchor
namespace _project.scripts.Core.CombatSystem.Projectiles
{
    public abstract class ProjectileSetting : ScriptableObject
    {
        public Sprite sprite;
        public int damageAmount;
        public int penetrationAmount;
    }
}