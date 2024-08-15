using System;
using _project.scripts.Core.CombatSystem.Projectiles;
using UnityEngine;

namespace _project.scripts.Core.CombatSystem.MagazineSystem
{
    [Serializable]
    public struct MagazineParams
    {
        public MagazineSo magazineSo;
        public AmmunitionSo ammunition;
    }
    public class Magazine
    {
        public MagazineSo MagazineSettings { get; }
        public AmmunitionSo FilledAmmo { get; }

        private int _remain;

        public static Magazine Create(MagazineParams parameters)
        {
            if (parameters.magazineSo.caliberRequired != parameters.ammunition.caliberSo)
            {
                Debug.LogError("Magazine : Not the same caliber !!");
                return null;
            }
            return new Magazine(parameters.magazineSo, parameters.ammunition);
        }
        
        public Magazine(MagazineSo setting, AmmunitionSo filledWith)
        {
            if (setting.caliberRequired != filledWith.caliberSo)
            {
                Debug.LogError("Magazine : Not the same caliber !!");
                return;
            }
            MagazineSettings = setting;
            FilledAmmo = filledWith;

            _remain = setting.capacity;
        }   

        public bool CanFire()
        {
            return _remain-- > 0;
        }
        
        public int Capacity =>MagazineSettings.capacity;
        public int Damage => Mathf.FloorToInt(FilledAmmo.caliberSo.damageAmount * FilledAmmo.damageModifier);
        public int Penetration => Mathf.FloorToInt(FilledAmmo.caliberSo.penetrationAmount * FilledAmmo.penetrationModifier);
        public float Speed => FilledAmmo.caliberSo.bulletSpeed * FilledAmmo.speedModifier;
    }
}