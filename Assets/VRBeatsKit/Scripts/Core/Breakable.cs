using DamageSystem;
using UnityEngine;
using VRBeats.Events;

namespace VRBeats
{
    public class Breakable : Damageable
    {        
        [SerializeField] private float defaultExplosionRadius = 2.0f;
        [SerializeField] private float forceModifier = 10.0f;
        [SerializeField] private float linealForceModifier = 5.0f;
        [SerializeField] private OnDamageEvent onBreak = null;

        private Rigidbody[] shatterPieces = null;

        private void Awake()
        {
            shatterPieces = GetComponentsInChildren<Rigidbody>();
        }

        public void Break(DamageInfo info)
        {
            for (int n = 0; n < shatterPieces.Length; n++)
            {
                shatterPieces[n].transform.parent = null;
                shatterPieces[n].isKinematic = false;
                shatterPieces[n].AddExplosionForce(info.hitForce * forceModifier, info.hitPoint , defaultExplosionRadius);
                shatterPieces[n].AddForce(info.hitDir * info.hitForce * linealForceModifier);
            }

            //Debug.Break();
        }

        public override void DoDamage(DamageInfo info)
        {
            Break(info);
            onBreak.Invoke(info);
        }
    }

}

