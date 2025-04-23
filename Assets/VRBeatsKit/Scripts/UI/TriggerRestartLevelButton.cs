using UnityEngine;
using UnityEngine.UI;
using VRBeats.ScriptableEvents;

namespace VRBeats.UI
{
    [RequireComponent(typeof(Button))]
    public class TriggerRestartLevelButton : MonoBehaviour
    {
        [SerializeField] private GameEvent onRestart = null;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener( TriggerRestartEvent ); ;
        }

        private void TriggerRestartEvent()
        {
            onRestart.Invoke();
        }
    }
}

