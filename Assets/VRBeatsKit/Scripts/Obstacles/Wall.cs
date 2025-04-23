using UnityEngine;
using VRBeats.ScriptableEvents;

namespace VRBeats
{
    public class Wall : MonoBehaviour
    {
        [SerializeField] private GameEvent onInsideDeadZone = null;
        [SerializeField] private GameEvent onGameOver = null;
        [SerializeField] private float deadTime = 2.0f;

        private Spawneable spawneable = null;
        private float timer = 0.0f;
        private bool canKillPlayer = true;
        private Transform player = null;
        private bool canBeKilled = true;
        private bool destroyed = false;

        private void Awake()
        {
            player = VR_BeatManager.instance.Player.transform;
            spawneable = GetComponent<Spawneable>();
        }

        private void OnDestroy()
        {
            destroyed = true;
        }

        private void Update()
        {
            transform.position += Vector3.forward * spawneable.Speed * Time.deltaTime;

            if (ShouldKill())
            {
                Kill();
            }

        }
                
        private void OnTriggerEnter(Collider other)
        {
            timer = 0.0f;
            onInsideDeadZone.Invoke();          
        }

        private void OnTriggerStay(Collider other)
        {
            timer += Time.fixedDeltaTime;
           
            if (timer >= deadTime && canKillPlayer)
            {               
                canKillPlayer = false;
                onGameOver.Invoke();
            }

        }

        private bool ShouldKill()
        {
            return canBeKilled && transform.position.z < player.position.z - 5.0f;
        }

        public void Kill()
        {
            canBeKilled = false;
            transform.ScaleTween(Vector3.zero, 2.0f).SetEase(Ease.EaseOutExpo).SetOnComplete(delegate
            {
                if(!destroyed)
                    Destroy(gameObject);
            });
        }


    }
}

