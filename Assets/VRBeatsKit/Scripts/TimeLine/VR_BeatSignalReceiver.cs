
using UnityEngine;
using UnityEngine.Playables;

namespace VRBeats
{
    public class VR_BeatSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        private EnviromentController enviromentController = null;

        private double lastTime = 0;
        private int frame = 0;

        private void Awake()
        {
            enviromentController = FindObjectOfType<EnviromentController>();
        }

        private void Update()
        {
            frame++;
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is VR_BeatSpawnMarker spawnMarker)
            {
                VR_BeatManager.instance.Spawn(spawnMarker.spawneable, spawnMarker.spawInfo);

                lastTime = spawnMarker.time;

            }
            else if (notification is VR_BeatEnvironmentColorMarker enviromentColorMarker)
            {
                enviromentController.FadeToColor(enviromentColorMarker.color, enviromentColorMarker.fadeTime, enviromentColorMarker.ease);
            }
            else if ( notification is VR_BeatEnvironmentEmissionMarker emmmisionMarker )
            {
                enviromentController.FadeEmmisiveValue(emmmisionMarker.targetEmissionValue, emmmisionMarker.fadeTime, emmmisionMarker.ease);
            }

            
        }
    }
}
