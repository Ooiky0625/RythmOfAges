using UnityEngine;

namespace VRBeats
{
    public class VR_BeatSpawnMarker : VR_BeatMarker
    {
        [SerializeField] public Spawneable spawneable = null;
        [SerializeField] [HideInInspector] public SpawnEventInfo spawInfo = null;
    }

}
