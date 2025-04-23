using UnityEngine;

namespace VRBeats.ScriptableEvents
{
    [CreateAssetMenu(fileName = "IntEvent" , menuName = "VR Beats/Events/Int", order =50)]
    public class IntGameEvent : BaseGameEvent<IntEventListener , int>
    {
       
    }
}

