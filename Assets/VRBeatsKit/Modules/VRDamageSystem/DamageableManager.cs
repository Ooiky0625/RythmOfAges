using DamageSystem.Events;
using UnityEngine;
using UnityEngine.Events;

namespace DamageSystem
{
    public enum DamageType
    {
        Explosion,
        Shoot,
        Physical
    }


    public class DamageableManager : MonoBehaviour
    {
        
        [SerializeField] protected float hp = 100.0f;
        [SerializeField] private float regenerationSpeed = 0.0f;
        [SerializeField] protected Rigidbody com = null;
        [SerializeField] private OnDamageEvent onDamage = null;
        [SerializeField] private UnityEvent onDie = null;
        [SerializeField] protected OnValueChangeEvent onHPChangeEvent;

        protected bool isDead = false;
        private float maxHP = 0.0f;

        public float HP { get { return hp; } }
        public OnDamageEvent OnDamage { get { return onDamage; } }
        public OnValueChangeEvent OnHPChangeEvent { get { return onHPChangeEvent; }  }
        public UnityEvent OnDie { get { return onDie; } }
        public bool Invulnerable { get; set; }
        public bool IsDead { get { return isDead; } }
        public float MaxHP { get { return maxHP; } }

        
        protected virtual void Awake()
        {
            maxHP = hp;

            SetDamageablePartsOwner();
        }

        protected virtual void Update()
        {
            DoRegeneration();
        }

        protected virtual void DoRegeneration()
        {
            if(regenerationSpeed > 0.0f && !isDead)
            {
                ModifyHP( regenerationSpeed * Time.deltaTime );
            }
        }

        protected virtual void SetDamageablePartsOwner()
        {
            //get all damageable
            DamageablePart[] damageableArray = GetComponentsInChildren<DamageablePart>();

            //set his owner
            for (int n = 0; n < damageableArray.Length; n++)
            {              
                damageableArray[n].SetOwner( this );
            }
        }

        public virtual void DoDamage(DamageInfo info, DamageablePart damageable)
        {

            //if we are dead just apply the impact force
            if (isDead)
            {
                ApplyImpactForce( info, damageable );

                if (onDamage != null)
                    onDamage.Invoke( info , damageable );
                return;
            }

            ModifyHP(-info.dmg);

            if (hp <= 0.0f)
            {
                TriggerDieEvent();
                ApplyImpactForce( info, damageable );
            }

            if (onDamage != null)
            {
                onDamage.Invoke( info, damageable );
            }

        }
        
        protected virtual void ApplyImpactForce(DamageInfo info, DamageablePart damageable)
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

        protected virtual void TriggerDieEvent()
        {
            if (onDie != null)
                onDie.Invoke();
            
            isDead = true;
        }

        public void SetupDamageableLimbs()
        {
            Collider[] colliderArray = transform.GetComponentsInChildren<Collider>();

            for (int n = 0; n < colliderArray.Length; n++)
            {
                if (colliderArray[n].GetComponent<DamageableLimb>() == null)
                {
                    colliderArray[n].gameObject.AddComponent<DamageableLimb>();
                }
            }
        }

        public virtual void Kill()
        {
            ModifyHP(float.MinValue);

            if (onDie != null)
                onDie.Invoke();
        }

        public virtual void ModifyHP(float v, bool triggerEvent = true)
        {
            hp += v;

            if (hp < 0.0f)
                hp = 0.0f;
            if (hp > MaxHP)
                hp = maxHP;

            if (triggerEvent)
            {
                onHPChangeEvent.Invoke(hp);
            }
        }
    }
}

