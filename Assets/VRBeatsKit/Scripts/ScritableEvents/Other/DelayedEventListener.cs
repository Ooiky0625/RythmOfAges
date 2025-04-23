using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBeats.ScriptableEvents 
{
    public class DelayedEventListener : EventListener
    {
        [SerializeField] private float delay = 0.0f;

        private Coroutine delayedInvokeRoutine = null;

        public override void Invoke()
        {
            if (delayedInvokeRoutine != null)
                StopCoroutine( delayedInvokeRoutine );

            delayedInvokeRoutine = StartCoroutine( DelayedInvokeRoutine() );
        }

        private IEnumerator DelayedInvokeRoutine()
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("Delayed invoke");
            base.Invoke();
        }

    }

}

