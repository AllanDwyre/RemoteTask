using _project.scripts.Core.CombatSystem;
using _project.scripts.Core.CombatSystem.Projectiles;
using UnityEditor;
using UnityEngine;

namespace _project.EditorScripts.Nameable
{
    [CustomEditor(typeof(AmmunitionSo))]
    public class AmmunitionSoEditor : BaseNameableEditor<AmmunitionSo>
    {
        protected override string PropertyName => "ammunitionName";
        protected override Object AssetPreview => null;
    }
    
    [CustomEditor(typeof(CaliberSo))]
    public class CaliberSoEditor : BaseNameableEditor<CaliberSo>
    {
        protected override string PropertyName  => "caliberName";
        protected override Object AssetPreview => (target as CaliberSo).sprite;
    }
    
    [CustomEditor(typeof(WeaponSettings))]
    public class WeaponSettingsEditor : BaseNameableEditor<WeaponSettings>
    {
        protected override string PropertyName  => "weaponName";
        protected override Object AssetPreview => (target as WeaponSettings).sprite;
    }
}