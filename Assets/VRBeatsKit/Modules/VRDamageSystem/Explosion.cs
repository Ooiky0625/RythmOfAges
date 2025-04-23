using UnityEngine;
using VRSDK;

namespace DamageSystem
{
    public class Explosion : MonoBehaviour
    {
        #region INSPECTOR
        [SerializeField] private float dmg = 200.0f;        
        [SerializeField] private float explosionRange = 0.0f;
        [SerializeField] private float explosionForce = 0.0f;
        [SerializeField] private float upwardsModifier = 0.0f;
        [SerializeField] private bool autoExplode = true;
        #endregion

        private DamageInfo damageInfo = new DamageInfo();

        private void Awake()
        {
            if (autoExplode)
            {
                Explode();
            }
        }
        
        

        public virtual void Explode(GameObject sender = null)
        {           
            Collider[] colliderArray = Physics.OverlapSphere( transform.position, explosionRange );
            
            for (int n = 0; n < colliderArray.Length; n++)
            {
                Damageable damageable = colliderArray[n].GetComponent<Damageable>();

                if (damageable != null)
                {
                    float distance = Vector3.Distance(transform.position, damageable.transform.position);
                    float distanceFactor = Mathf.Abs((distance / explosionRange) - 1.0f); // a distance factor from 0.0f to 1.0f
                   
                    //create damage info
                    DamageInfo info = CreateDamageInfo(distanceFactor);
                    info.sender = sender;
                   

                    //send damage event
                    DoDamage(damageable, info);
                }
                else
                {                  

                    Rigidbody rb = colliderArray[n].GetComponent<Rigidbody>();

                    if (rb != null)
                    {
                        ApplyImpactForce( rb );
                    }
                    else
                    {
                        VR_Grabbable grabbable = VR_Manager.instance.GetGrabbableFromCollider( colliderArray[n] );

                        if (grabbable != null && grabbable.RB != null)
                        {
                            ApplyImpactForce( grabbable.RB );
                        }
                    }
                }
            }


            Destroy( gameObject  , 5.0f);
        }

        private DamageInfo CreateDamageInfo(float distanceFactor)
        {
            if (damageInfo != null)
            {
                damageInfo = new DamageInfo();
            }

            damageInfo.hitForce = explosionForce;
            damageInfo.hitPoint = transform.position;
            damageInfo.explosionRadius = explosionRange;
            damageInfo.upwardsModifier = upwardsModifier;
            damageInfo.dmg = dmg * distanceFactor;
            damageInfo.damageType = DamageType.Explosion;

            return damageInfo;
        }

        protected virtual void ApplyImpactForce(Rigidbody rb)
        {
            rb.AddExplosionForce( explosionForce, transform.position, explosionRange, upwardsModifier );
        }

        protected virtual void DoDamage(Damageable damageable, DamageInfo info)
        {
            damageable.DoDamage(info);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere( transform.position, explosionRange );
        }
    }

}

