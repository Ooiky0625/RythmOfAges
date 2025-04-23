using UnityEngine;

namespace VRSDK
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform pockectsAnchorPoint = null;
        [SerializeField] private VR_ScreenFader gameOverScreenFader = null;

        public Transform PocketsAnchorPoint { get { return pockectsAnchorPoint; } }
        public VR_ScreenFader GameOverScreenFader { get { return gameOverScreenFader; } }
    }
}

