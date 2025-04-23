using UnityEngine.Events;

namespace DamageSystem.Events
{
    [System.Serializable]
    public class OnDamageEvent : UnityEvent<DamageInfo , DamageablePart> { }

}