using UnityEngine;

namespace VRSDK
{
    public class DropZoneInfo : MonoBehaviour
    {
        [SerializeField] private Vector3 positionOffset = Vector3.zero;
        [SerializeField] private Vector3 rotationOffset = Vector3.zero;
        [SerializeField] private float scaleModifier = 0.0f;

        public Vector3 OriginalScale { get { return originalScale; } }
        public Vector3 PositionOffset { get { return positionOffset; } }
        public Vector3 RotationOffset { get { return rotationOffset; } }
        public float ScaleModifier { get { return scaleModifier; } }

        private Vector3 originalScale = Vector3.zero;

        private void Awake()
        {
            originalScale = transform.localScale;
        }
    }

}

