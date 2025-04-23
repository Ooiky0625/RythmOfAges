using UnityEngine;

namespace VRBeats
{
    public class VR_BeatEnvironmentEmissionMarker : VR_BeatMarker
    {
        [SerializeField] public float targetEmissionValue = 100.0f;
        [SerializeField] public float fadeTime = 1.5f;
        [SerializeField] public Ease ease = Ease.EaseOutExpo;
    }

}

