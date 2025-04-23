using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRBeats.ScriptableEvents
{
    public abstract class BaseEventListener<T1 , T2 ,T3 , T4> : MonoBehaviour , 
        IExposeInvoke<T2>  
        where T1 : 
        IExposeAddListener<T3> , 
        IExposeRemoveListener<T3> 
        where T4: UnityEvent<T2>
        
    {

        [SerializeField] private T1 gameEvent;
        [SerializeField] private T4 response;       

        public T4 Response { get { return response; } }
        public T1 GameEvent { get { return gameEvent; } }

        private void Awake()
        {
            if (gameEvent != null)
            {               
                gameEvent.AddListener( (T3) ((System.Object)this) );
            }
        }

        private void OnDisable()
        {
            if (gameEvent != null)
            {
                gameEvent.RemoveListener((T3)((System.Object)this));
            }
        }

        private void OnDestroy()
        {
            if (gameEvent != null)
            {
                gameEvent.RemoveListener( (T3)((System.Object)this) );
            }
        }


        public void Invoke(T2 value)
        {
            if(response != null)
                response.Invoke(value);
        }

        public void AddEventListener(UnityAction<T2> unityAction)
        {           
            response.AddListener( unityAction );
        }

        public void SetEventListener(T1 gameEvent)
        {
            this.gameEvent = gameEvent;
        }

       
    }
    



}
