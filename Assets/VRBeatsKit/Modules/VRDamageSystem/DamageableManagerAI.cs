using UnityEngine;

namespace DamageSystem
{
    public class DamageableManagerAI : DamageableManager
    {
        protected override void Awake()
        {
            base.Awake();

            OnDamage.AddListener( ApplyImpactForce );
        }

        private void ApplyImpactForce(DamageInfo info, DamageablePart damageable)
        {
            //if this AI if no dead it is being controlled by the Animator so dont apply any impact force
            if (!IsDead)
                return;

            if (com != null && info.damageType == DamageType.Explosion)
            {
                com.AddExplosionForce( info.hitForce, info.hitPoint, info.explosionRadius, info.upwardsModifier, info.forceMode );
            }
            else if (damageable.RB != null && info.damageType == DamageType.Shoot)
            {
                damageable.RB.AddForceAtPosition( info.hitForce * info.hitDir, info.hitPoint, info.forceMode );
            }
        }
    }
}

