using UnityEngine;
using UnityEngine.Events;

namespace VRBeats.ScriptableEvents
{
    public class FloatEventListener : BaseEventListener<FloatGameEvent , float , FloatEventListener , OnFloatValueChange>
    {

    }

    [System.Serializable]
    public class OnFloatValueChange : UnityEvent<float> { }

}


