using UnityEngine;

namespace VRBeats
{
    [CreateAssetMenu(menuName = "VR Beats/Create Settings" , fileName = "Settings")]
    public class VR_BeatSettings : ScriptableObject
    {
        [Header("Core")]        
        [SerializeField] private Color rightColor = Color.magenta;
        [SerializeField] private Color leftColor = new Color(0.5f, 0f, 0.5f);
        [SerializeField] private float glowIntensity = 5.0f;
        [SerializeField] private float targetTravelDistance = 10.0f;
        [SerializeField] private float targetTravelTime = 0.2f;
        [SerializeField] private Ease targetTravelEase = Ease.EaseInOutSine;
        [SerializeField] private int errorLimit = 5;
        [Header("Score")]
        [SerializeField] private int scorePerHit = 50;
        [SerializeField] private int maxMultiplier = 8;
       
        public Color RightColor { get { return rightColor; } }
        public Color LeftColor { get { return leftColor; } }
        public float GlowIntensity { get { return glowIntensity; } }
        public float TargetTravelDistance { get { return Mathf.Abs( targetTravelDistance ); } }
        public float TargetTravelTime { get { return targetTravelTime; } }
        public Ease TargetTravelEase { get { return targetTravelEase; } }   
        public int ScorePerHit { get { return scorePerHit; } }
        public int MaxMultiplier { get { return maxMultiplier; } } 
        public int ErrorLimit { get { return errorLimit; } }
    }

}
