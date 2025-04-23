using UnityEngine;

namespace DamageSystem
{
    //this script details about how should a bullet bounce over this object
    public class SurfaceDetails : Damageable
    {
        [SerializeField] private bool bulletsCanBounce = false;
        [SerializeField] private bool stickArrows = true;
        [SerializeField] private float bulletsSpeedLoseOnBounce = 0.20f;
        [SerializeField] private AudioClip[] hitSoundArray = null;
        [SerializeField] [Range(0.0f , 1.0f)] private float soundvolume = 1.0f;
        [SerializeField] private GameObject[] hitEffectArray = null;
        [SerializeField] private float lifeTime = 0.0f;
        [SerializeField] private bool parentEffect = false;
        [SerializeField] private Vector3 rotOffset = Vector3.zero;
        [SerializeField] private float effectMinDelay = 0.0f;
        [SerializeField] private float effectMaxDelay = 0.0f;

        public bool BulletsCanBounce { get { return bulletsCanBounce; } }
        public bool StickArrows { get { return stickArrows; } }
        public float BulletsSpeedLoseOnBounce { get { return bulletsSpeedLoseOnBounce; } }
        
        private AudioSource audioSource = null;
        private float nextEffectTime = 0.0f;

        public void CopySettings(SurfaceDetails surface)
        {
            bulletsCanBounce = surface.bulletsCanBounce;
            bulletsSpeedLoseOnBounce = surface.bulletsSpeedLoseOnBounce;
            hitSoundArray = surface.hitSoundArray;
            hitEffectArray = surface.hitEffectArray;
            soundvolume = surface.soundvolume;
            lifeTime = surface.lifeTime;
            parentEffect = surface.parentEffect;
            rotOffset = surface.rotOffset;
            effectMinDelay = surface.effectMinDelay;
            effectMaxDelay = surface.effectMaxDelay;
            
        }

        public override void DoDamage(DamageInfo info)
        {
            if(nextEffectTime > Time.time)
                return;

            nextEffectTime = Random.Range(effectMinDelay, effectMaxDelay) + Time.time;
            
            InstantiateHitEffect(info.hitPoint , Quaternion.LookRotation( info.hitDir ) * Quaternion.Euler(rotOffset));
            PlayHitSound();
        }

        private void PlayHitSound()
        {
            if (hitSoundArray == null || hitSoundArray.Length == 0)
                return;

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();

                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.volume = Random.Range(soundvolume/2.0f , soundvolume);
            audioSource.pitch = Random.Range(0.7f , 1.0f);
            audioSource.clip = hitSoundArray[ Random.Range(0 , hitSoundArray.Length) ];

            audioSource.Play();

        }

        private void InstantiateHitEffect(Vector3 position , Quaternion rotation)
        {
            if (hitEffectArray == null || hitEffectArray.Length == 0)
                return;

            GameObject go = Instantiate( hitEffectArray[Random.Range( 0, hitEffectArray.Length )], position, rotation);
            Destroy( go, lifeTime );

            if (parentEffect)
            {
                go.transform.parent = transform;
            }
        }

    }
}

