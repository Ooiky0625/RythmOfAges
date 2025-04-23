using System.Collections.Generic;
using UnityEngine;

namespace VRBeats.ScriptableEvents
{
    public class BaseGameEvent<T1 , T2> : ScriptableObject , IExposeAddListener<T1>, IExposeRemoveListener<T1> , IExposeInvoke<T2> where T1 : IExposeInvoke<T2> 
    {
        [SerializeField] private List<T1> eventListenerList = null;
        [HideInInspector] public T2 invokeValue;

        public List<T1> EventListenerList { get { return eventListenerList; } }


        public void AddListener(T1 listener)
        {
            eventListenerList.Add(listener);
        }

        public void RemoveListener(T1 listener)
        {
            eventListenerList.Remove(listener);
        }

        public void Invoke(T2 value)
        {
            for (int n = eventListenerList.Count - 1; n >= 0; n--)
            {
                eventListenerList[n].Invoke(value);
            }
        }

    }

}
