using UnityEngine;
using UnityEngine.Events;

namespace VRBeats.ScriptableEvents
{
    public class IntEventListener : BaseEventListener<IntGameEvent , int , IntEventListener , OnIntValueChange>
    {

    }

    [System.Serializable]
    public class OnIntValueChange : UnityEvent<int> { }

}


