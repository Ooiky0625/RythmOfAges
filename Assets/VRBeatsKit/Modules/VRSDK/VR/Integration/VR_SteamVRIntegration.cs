using UnityEngine;

namespace VRSDK.Integration
{
    public class VR_SteamVRIntegration : VR_Integration
    {
        public override VR_Controller GetActiveController()
        {
            return VR_Manager.instance.Player.RightController;
        }

        public override Transform GetLeftHandTransform()
        {
            return GameObject.Find("LeftHand").transform;
        }

        public override Transform GeTrackingSpaceTransform()
        {
            return GameObject.Find("SteamVRObjects").transform;
        }

        public override Transform GetRightHandTransform()
        {
            return GameObject.Find("RightHand").transform;
        }
    }

}
