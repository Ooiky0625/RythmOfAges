using UnityEngine;

namespace VRSDK
{
    public class DropZoneOffset : MonoBehaviour
    {
        [SerializeField] private Vector3 positionOffset = Vector3.zero;
        [SerializeField] private Vector3 rotationOffset = Vector3.zero;

        public Vector3 PositionOffset { get { return positionOffset; } }
        public Vector3 RotationOffset { get { return rotationOffset; } }

    }
}

