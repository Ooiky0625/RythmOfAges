using System;
using DamageSystem;
using UnityEngine;
using VRSDK;

namespace VRBeats
{
    public class DamageSaber : VR_MeleeWeapon
    {
        [SerializeField] private ColorSide colorSide;

        private VR_Controller controller;

        private void Start()
        {
            if (colorSide == ColorSide.Left) controller = VR_Manager.instance.Player.LeftController;
            if (colorSide == ColorSide.Right) controller = VR_Manager.instance.Player.RightController;
        }

        protected override DamageInfo CreateDamageInfo(Vector3 hitPoint)
        {
            var damageInfo = base.CreateDamageInfo(hitPoint);
            BeatDamageInfo beatDamageInfo = new BeatDamageInfo(damageInfo);

            Vector3 controllerVelocity = controller.Velocity;

            beatDamageInfo.hitForce = Mathf.Min((controllerVelocity * hitForce).magnitude, maxHitForce);
            beatDamageInfo.hitObject = gameObject;
            beatDamageInfo.colorSide = colorSide;
            beatDamageInfo.velocity = controller.Velocity.magnitude;

            return beatDamageInfo;
        }
    }
}

