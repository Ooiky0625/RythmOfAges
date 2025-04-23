using UnityEngine;
using UnityEngine.AI;


namespace VRSDK
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies = null;
        [SerializeField] private float spawnTime = 5.0f;
        [SerializeField] private float spawnRadius = 1.0f;
        [SerializeField] private int maxEnemies = 10;
        

        private float timer = 0.0f;
        
        private void Awake()
        {
            timer = Random.Range(spawnTime / 2.0f , spawnTime);
        }

        private void Update()
        {
            timer -= Time.deltaTime;

            if(timer <= 0.0f)
                Spawn();
        }

        //creates a new enemy
        private void Spawn()
        {            
            timer = Random.Range(spawnTime / 2.0f, spawnTime);

            //check if dont have to many enemies already
            if (GameObject.FindGameObjectsWithTag( "Enemy" ).Length > maxEnemies)
                return;

            GameObject enemy = enemies[Random.Range(0 , enemies.Length)];

            if (RandomNavmeshLocation(spawnRadius, out Vector3 spawnPoint))
            {
                Instantiate(enemy, spawnPoint, transform.rotation);
            }
        }


        //return a random point inside the navmesh radius
        private bool RandomNavmeshLocation(float radius, out Vector3 spawnPosition)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            NavMeshHit hit;
            spawnPosition = Vector3.zero;
            
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                spawnPosition = hit.position;
                return true;
            }

            return false;
        }


    }

}

