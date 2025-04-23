using UnityEngine;

namespace VRBeats.ScriptableEvents
{
    public class TriggerScritableEventOnAwake : MonoBehaviour
    {
        [SerializeField] private GameEvent gameEvent = null;

        private void Start()
        {           
            gameEvent.Invoke();
        }


    }

}
