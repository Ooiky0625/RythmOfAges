using UnityEngine;

namespace VRBeats.ScriptableEvents
{
    [CreateAssetMenu(fileName = "FloatEvent" , menuName = "VR Beats/Events/Float", order =50)]
    public class FloatGameEvent : BaseGameEvent<FloatEventListener , float>
    {
       
    }
}

