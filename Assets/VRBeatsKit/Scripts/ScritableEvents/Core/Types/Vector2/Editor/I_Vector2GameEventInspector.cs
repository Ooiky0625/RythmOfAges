using UnityEditor;
using UnityEngine;

namespace VRBeats.ScriptableEvents
{
    [CustomEditor(typeof(Vector2GameEvent))]
    public class I_Vector2GameEventInspector : BaseGameEventInspector<Vector2GameEvent , Vector2, Vector2EventListener, OnVector2ValueChange>
    {

    }

}

