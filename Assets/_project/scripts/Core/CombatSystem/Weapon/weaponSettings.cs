using _project.scripts.Utils;
using UnityEngine;

namespace _project.scripts.Core.CombatSystem
{
    // TODO : next level + documentation :
    // https://rimworldwiki.com/wiki/Weapons
    // https://combatextended.fandom.com/wiki/Features
    // https://github.com/CombatExtendedRWMod/CombatExtended/tree/master/Source/CombatExtended
    
    [CreateAssetMenu(menuName = "ScriptableObject/Combat/WeaponSetting", fileName = "defaultWeapon")]
    public class WeaponSettings : ScriptableObject
    {
        [Header("Visual")]
        public string weaponName;
        public Sprite sprite;

        [Header("Basics")]
        public CaliberSo caliberSo;
        public int maxRange;
        public bool isSecondaryWeapon;
        public Vector3 muzzlePoint;

        [Header("Times stats")]
        public float warmUp;
        public float cooldown;
        public float reloadTime;
        
        [Header("Accuracy")]
        [Range(0f,0.5f)]
        public float spread;
        [Tooltip("A weapon sway represent the difficulty to use it, it's in degrees and can be change based on diverse facts")]
        public float sway;
        
        public Optional<float> stoppingPower;

        [Header("Burst")]
        public int burstCount = 1;
        [Tooltip("Delay between each bullets fired from a same burst")]public float burst;
    }
}