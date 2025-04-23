using UnityEditor;

namespace VRBeats.ScriptableEvents
{
    [CustomEditor(typeof(IntGameEvent))]
    public class I_IntGameEventInspector : BaseGameEventInspector<IntGameEvent , int , IntEventListener , OnIntValueChange>
    {

    }

}

