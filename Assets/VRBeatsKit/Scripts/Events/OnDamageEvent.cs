using DamageSystem;
using UnityEngine.Events;

namespace VRBeats.Events
{
    [System.Serializable]
    public class OnDamageEvent : UnityEvent<DamageInfo>
    {
       
    }
}
