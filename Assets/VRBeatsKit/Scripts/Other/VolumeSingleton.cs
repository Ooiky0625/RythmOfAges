using UnityEngine;


namespace VRBeats
{
    public class VolumeSingleton : MonoBehaviour
    {
        private static VolumeSingleton Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                VolumeSingleton[] instancesArray = FindObjectsOfType<VolumeSingleton>();

                foreach (var volumeSingleton in instancesArray)
                {
                    if (volumeSingleton != Instance)
                    {
                        Destroy(volumeSingleton.gameObject);
                    }
                }
            }
        }
    }

}

