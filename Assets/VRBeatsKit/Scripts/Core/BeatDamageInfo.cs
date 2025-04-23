using DamageSystem;
using UnityEngine;

namespace VRBeats
{
    public class BeatDamageInfo : DamageInfo
    {
        public float velocity;
        public ColorSide colorSide;
        public GameObject hitObject;

        public BeatDamageInfo(DamageInfo damageInfo)
        {
            dmg = damageInfo.dmg;
            hitDir = damageInfo.hitDir;
            hitPoint = damageInfo.hitPoint;
            explosionRadius = damageInfo.explosionRadius;
            upwardsModifier = damageInfo.upwardsModifier;
            hitForce = damageInfo.hitForce;
            forceMode = damageInfo.forceMode;
            damageType = damageInfo.damageType;
            canDismember = damageInfo.canDismember;
            sender = damageInfo.sender;
        }
    }
}

