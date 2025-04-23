using DamageSystem.Dismember;
using UnityEngine.Events;

namespace DamageSystem.Events
{   
    [System.Serializable]
    public class OnDismemberEvent : UnityEvent<DismemberPart> { }

}