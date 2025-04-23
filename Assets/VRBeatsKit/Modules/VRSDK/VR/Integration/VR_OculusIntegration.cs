using UnityEngine;

namespace VRSDK.Integration
{
    public class VR_OculusIntegration : VR_Integration
    {
        public override VR_Controller GetActiveController()
        {
            return VR_Manager.instance.Player.RightController;
        }

        public override Transform GetLeftHandTransform()
        {
            return GameObject.Find("LeftHandAnchor").transform;
        }

        public override Transform GeTrackingSpaceTransform()
        {
            return GameObject.Find("TrackingSpace").transform;
        }

        public override Transform GetRightHandTransform()
        {
            return GameObject.Find("RightHandAnchor").transform;
        }
    }
}

