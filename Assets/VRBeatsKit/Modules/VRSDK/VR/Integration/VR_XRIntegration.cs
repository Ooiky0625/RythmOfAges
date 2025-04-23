using UnityEngine;

namespace VRSDK.Integration
{
    public class VR_XRIntegration : VR_Integration
    {
        public override VR_Controller GetActiveController()
        {
            return VR_Manager.instance.Player.RightController;
        }

        public override Transform GetLeftHandTransform()
        {
            return VR_Manager.instance.Player.LeftController.OriginalParent;
        }

        public override Transform GetRightHandTransform()
        {
            return VR_Manager.instance.Player.RightController.OriginalParent;
        }

        public override Transform GeTrackingSpaceTransform()
        {
            var cameraOffset = GameObject.Find("CameraOffset");
            if (cameraOffset == null) return null;
            
            return cameraOffset.transform;
        }

        
    }

}

