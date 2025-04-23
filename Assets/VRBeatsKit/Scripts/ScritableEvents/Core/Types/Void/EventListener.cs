using UnityEngine;
using UnityEngine.Events;
using System;

namespace VRBeats.ScriptableEvents
{
    public class EventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent gameEvent = null;
        [SerializeField] private UnityEvent response = null;        

        public UnityEvent Response { get { return response; } }
                

        private void Awake()
        {
            if (gameEvent != null)
            {
                gameEvent.AddListener(this);
            }
        }

        private void OnDisable()
        {
            if (gameEvent != null)
            {
                gameEvent.RemoveListener(this);
            }
        }

        private void OnDestroy()
        {
            if (gameEvent != null)
            {
                gameEvent.RemoveListener(this);
            }
        }

        private void OnSpawn()
        {
            if (gameEvent != null)
            {
                gameEvent.AddListener(this);
            }
        }


        public virtual void Invoke()
        {
            response.Invoke();
        }
    }

}
