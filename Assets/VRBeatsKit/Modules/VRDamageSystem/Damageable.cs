using UnityEngine;

namespace DamageSystem
{
    /// <summary>
    /// Basic class for all damageable stuff
    /// </summary>
    public abstract class Damageable : MonoBehaviour
    {
        public abstract void DoDamage(DamageInfo info);
    }

}

