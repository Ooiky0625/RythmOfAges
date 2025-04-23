using DamageSystem;
using UnityEngine;
using VRBeats.ScriptableEvents;

namespace VRBeats
{
    public class Mine : MonoBehaviour
    {
        [SerializeField] private GameEvent onMineSlice = null;
        [SerializeField] private GameEvent onGameOver = null;

        private float speed = 0.0f;
        private Transform player = null;
        private bool canBeKilled = true;
        private bool destroyed = false;

        private void Start()
        {
            player = VR_BeatManager.instance.Player.transform;

            Spawneable spawneable = GetComponent<Spawneable>();
            speed = spawneable.Speed;

        }

        private void OnDestroy()
        {
            destroyed = true;
        }

        private void Update()
        {
            transform.position += Vector3.forward * speed * Time.deltaTime;

            if (ShouldKill())
            {
                Kill();
            }

        }

        public void OnCut( DamageInfo info )
        {
            onMineSlice.Invoke();
            onGameOver.Invoke();
        }

        private bool ShouldKill()
        {
            return canBeKilled && transform.position.z < player.position.z - 2.0f;
        }

        public void Kill()
        {            
            canBeKilled = false;
            transform.ScaleTween(Vector3.zero, 2.0f).SetEase(Ease.EaseOutExpo).SetOnComplete(delegate
            {
                if (!destroyed)
                    Destroy(gameObject);
            });
        }




    }

}


