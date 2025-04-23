using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace VRSDK
{

    /// this is just code for handling the demo scene logic, nothing important happens here
    public class DemoScene : MonoBehaviour
    {
        [SerializeField] private EnemySpawner[] enemySpawnerArray = null;
        [SerializeField] private GameObject spawnPrefab = null;
        [SerializeField] private Transform spawnPoint = null;
        [SerializeField] private Animator doorAnimator = null;
        

        private bool doorState = false;
        private bool canSetTrigger = true;
        private bool desireDoorState = false;
        private float currentLeverValue = 0.0f;
        private bool isSpawningEnemies = false;
        private VR_ScreenFader gameOverScreenFader = null;

        private void Start()
        {
            Player player = FindObjectOfType<Player>();
            gameOverScreenFader = player.GameOverScreenFader;
        }

        //called when the black button gets pressed
        public void OnButtonPressed()
        {
            Instantiate(spawnPrefab , spawnPoint.position + Random.insideUnitSphere , spawnPoint.rotation);           

        }

        private void Update()
        {
            //if we want to spawn enemies and the door is close open it
            if (isSpawningEnemies && !desireDoorState)
            {
                doorAnimator.SetTrigger( "Open" );
                return;
            }

            //if the lever is on the open zone
            if (currentLeverValue >= 0.9f)
            {
                if (!doorState && canSetTrigger)
                {
                    canSetTrigger = false;
                    desireDoorState = true;
                    doorAnimator.SetTrigger( "Open" );
                }

            }
            //if the lever is on the closed zone
            else if (currentLeverValue <= 0.1f)
            {
                if (doorState && canSetTrigger)
                {
                    canSetTrigger = false;
                    desireDoorState = false;
                    doorAnimator.SetTrigger( "Close" );
                }
            }
        }

        //called when the lever value change, in other words when you move the lever
        public void OnLeverValueChange(float v)
        {
            currentLeverValue = v;           
        }

        public void SetDoorState( bool state)
        {
            doorState = state;
            canSetTrigger = true;            
        }

        public void EnableEnemySpawning()
        {
            if (isSpawningEnemies)
                return;

            StartCoroutine( EnableEnemySpawningRoutine() );
        }

        private IEnumerator EnableEnemySpawningRoutine()
        {
            isSpawningEnemies = true;

            //the door is open start spawning right now
            if (doorState)
            {
                EnableAllEnemySpawners();
            }
            //the door is closed let's wait for it to open
            else
            {                
                yield return new WaitForSeconds(5.0f);
                EnableAllEnemySpawners();
            }
        }


        private void EnableAllEnemySpawners()
        {
            for (int n = 0; n < enemySpawnerArray.Length; n++)
            {
                enemySpawnerArray[n].enabled = true;
            }
        }

        public void GameOver()
        {
            gameOverScreenFader.FadeIn(1.5f , delegate 
            {
                SceneManager.LoadScene( SceneManager.GetActiveScene().name );
            } );
        }
        

    }
}

