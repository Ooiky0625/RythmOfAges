using UnityEngine;
using UnityEngine.Serialization;
using VRSDK;

namespace DamageSystem
{
    public class DamageablePart : Damageable
    {
        [FormerlySerializedAs("damageMultiplier")] 
        [SerializeField] private float m_damageMultiplier = 1.0f;
        [FormerlySerializedAs("rb")] 
        [SerializeField] private Rigidbody m_rb = null;

        private DamageableManager m_owner = null;


        public Rigidbody RB => m_rb;
        public DamageableManager Owner => m_owner;
        public float DamageMultiplier => m_damageMultiplier;

        private void Awake()
        {
            if(m_rb == null) m_rb = GetComponent<Rigidbody>();
        }

        public void ExternalSetup(float damageMultiplier, Rigidbody rb)
        {
            m_damageMultiplier = damageMultiplier;
            m_rb = rb;
        }

        public void SetOwner(DamageableManager owner)
        {
            this.m_owner = owner;
        }

        public override void DoDamage(DamageInfo info)
        {
            if (m_owner == null)
            {
                return;
            }

            info.dmg *= m_damageMultiplier;
            m_owner.DoDamage( info, this );
        }

        private void ProcessHit(Rigidbody rb , GameObject sender)
        {
            if (m_owner == null)
            {
                return;
            }
            
            DamageInfo info = new DamageInfo();
            info.damageType = DamageType.Physical;
            info.hitDir = rb.velocity.normalized;
            info.dmg = rb.velocity.magnitude * m_damageMultiplier;
            info.hitForce = rb.velocity.magnitude;
            info.sender = sender;

            DoDamage( info );
        }

        private void OnCollisionEnter(Collision other)
        {
            //in this way we can respond to hits from objects and apply damage,
            //like the player throwing a box to a enemy
            if (other.rigidbody != null)
            {
                VR_Grabbable grabbable = VR_Manager.instance.GetGrabbableFromCollider(other.collider);

                if (grabbable != null && grabbable.ObjectWasThrow && grabbable.LastInteractController != null)
                {                   
                    GameObject sender = grabbable.LastInteractController.transform.root.gameObject;
                    ProcessHit( other.rigidbody, sender );
                }
                else
                {
                    ProcessHit( other.rigidbody, other.gameObject );
                }
            }
        }
    }
}

