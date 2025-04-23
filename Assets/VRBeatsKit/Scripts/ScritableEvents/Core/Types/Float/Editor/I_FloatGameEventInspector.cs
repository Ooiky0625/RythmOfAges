using UnityEditor;

namespace VRBeats.ScriptableEvents
{
    [CustomEditor(typeof(FloatGameEvent))]
    public class I_FloatGameEventInspector : BaseGameEventInspector<FloatGameEvent , float , FloatEventListener , OnFloatValueChange>
    {

    }

}

