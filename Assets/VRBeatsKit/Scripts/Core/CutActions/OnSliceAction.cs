using DamageSystem;
using UnityEngine;


namespace VRBeats
{
    public abstract class OnSliceAction : ScriptableObject
    {
        public abstract bool OnSlice(VR_BeatCube beat, BeatDamageInfo info);        
    }

}

