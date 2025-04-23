using UnityEngine;

namespace VRBeats
{
    public class VR_BeatEnvironmentColorMarker : VR_BeatMarker
    {
        [SerializeField] public Color color = Color.black;
        [SerializeField] public float fadeTime = 1.5f;
        [SerializeField] public Ease ease = Ease.EaseOutExpo;
    }

}

