using UnityEngine;

namespace VRSDK.Integration
{
    public abstract class VR_Integration
    {        

        public VR_Controller RightController { get; }
        public VR_Controller LeftController { get; }

        public abstract Transform GetLeftHandTransform();
        public abstract Transform GetRightHandTransform();
        public abstract Transform GeTrackingSpaceTransform();

        public abstract VR_Controller GetActiveController();


    }

}
