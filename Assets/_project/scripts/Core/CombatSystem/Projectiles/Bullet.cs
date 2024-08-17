using _project.scripts.Core.CombatSystem.Weapon;
using UnityEngine;

namespace _project.scripts.Core.CombatSystem.Projectiles
{
    public class Bullet : Projectile
    {
        protected override ProjectileSetting ProjectileSo { get; set; }

        private AmmunitionSo _settings;

        public void Launch(Vector3 origin, Quaternion rotation, float shotSpeed, RangedWeapon launcher, AmmunitionSo settings)
        {
            Launch(origin, rotation, shotSpeed, launcher);
            Renderer.sprite = settings.caliberSo.sprite;
            ProjectileSo = settings.caliberSo;
            _settings = settings;
        }
    }
}
