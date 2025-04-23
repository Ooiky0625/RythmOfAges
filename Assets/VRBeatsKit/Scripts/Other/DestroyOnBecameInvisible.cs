using UnityEngine;

namespace VRBeats
{
    public class DestroyOnBecameInvisible : MonoBehaviour
    {
       private float maxLifeTime = 10.0f;

        private float timer = 0.0f;

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= maxLifeTime)
                Destroy(gameObject);

        }

        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }

}
