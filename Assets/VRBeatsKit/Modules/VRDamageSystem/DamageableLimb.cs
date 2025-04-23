using UnityEngine;

namespace DamageSystem
{
    /// <summary>
    /// This component report all limb damage to the the AI
    /// </summary>
    public class DamageableLimb : Damageable
    {        
        [SerializeField] private float damageMultiplier = 1.0f;        

        private DamageableManager owner = null;
        

        //public Rigidbody RB { get { return rb; } }
        public DamageableManager Owner { get { return owner; } }

       
        public void SetOwner(DamageableManager owner)
        {
            this.owner = owner;
        }

        public override void DoDamage(DamageInfo info)
        {
            info.dmg *= damageMultiplier;
            //owner.DoDamage(info, this);
        }

        private void ProcessHit(Rigidbody rb)
        {            
            DamageInfo info = new DamageInfo();
            info.damageType = DamageType.Physical;
            info.hitDir = rb.velocity.normalized;            
            info.dmg = rb.velocity.magnitude * damageMultiplier;
            info.hitForce = rb.velocity.magnitude;

            DoDamage(info);
            
        }

        private void OnCollisionEnter(Collision other)
        {
            //in this way we can respond to hits from objects and apply damage,
            //like the player throwing a box to a enemy
            if (other.rigidbody != null)
            {
                ProcessHit( other.rigidbody );
            }
            
        }

    }

}

