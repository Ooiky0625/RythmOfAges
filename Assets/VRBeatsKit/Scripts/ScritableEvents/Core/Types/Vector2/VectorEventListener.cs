using UnityEngine;
using UnityEngine.Events;

namespace VRBeats.ScriptableEvents
{
    public class Vector2EventListener : BaseEventListener<Vector2GameEvent, Vector2, Vector2EventListener, OnVector2ValueChange>
    {

    }

    [System.Serializable]
    public class OnVector2ValueChange : UnityEvent<Vector2> { }

}


