using System.Collections.Generic;
using UnityEngine;

namespace VRBeats.ScriptableEvents
{
    [CreateAssetMenu(fileName = "Event", menuName = "VR Beats/Events/Void", order = 10000)]
    public class GameEvent : ScriptableObject
    {
        [SerializeField] private List<EventListener> eventListenerList = null;       

        public List<EventListener> EventListenerList { get { return eventListenerList; } }      


        public void AddListener(EventListener listener)
        {
            if (!eventListenerList.Contains(listener))
            {
                eventListenerList.Add(listener);
            }
            
        }

        public void RemoveListener(EventListener listener)
        {
            if (eventListenerList.Contains(listener))
            {
                eventListenerList.Remove(listener);
            }
            
        }

        public void Invoke()
        {
            for (int n = eventListenerList.Count - 1; n >= 0; n--)
            {
                eventListenerList[n].Invoke();
            }
        }

    }

}
