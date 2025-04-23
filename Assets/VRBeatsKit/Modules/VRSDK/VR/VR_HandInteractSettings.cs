using UnityEngine;

namespace VRSDK
{
    [System.Serializable]
    public class VR_HandInteractSettings
    {
        public Transform interactPoint = null;
        public Transform highlightPoint = null;
        public Vector3 rotationOffset = Vector3.zero;
        public bool canInteract = true;

        public Vector3 CalculateGrabRotationOffset()
        {
            Vector3 handRotOffset = VR_Manager.instance.Player.HandGrabRotationOffset;
            return rotationOffset + handRotOffset;
        }
    }
}

