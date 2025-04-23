using UnityEngine;

namespace VRSDK
{
    public enum WeaponSelectionMenu
    {
        Reload = 0,
        Shoot,
        Damage,
        Recoil
    }

    [System.Serializable]
    public class VR_WeaponEditorPart 
    {
        public WeaponSelectionMenu selectedMenu = WeaponSelectionMenu.Reload;
        public bool foldoutBasicShootSettings = false;
        public bool foldoutOptionalShootSettings = false;
        public bool foldoutEffectsShootSettings = false;
        public bool foldoutSpreadDamageSettings = false;
    }
}

