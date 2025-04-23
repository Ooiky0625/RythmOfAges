using UnityEngine;
using UnityEngine.Events;
using VRSDK.Integration;

namespace VRSDK 
{
    public class VR_Player : MonoBehaviour
    {
        [SerializeField] private VR_Controller rightController = null;
        [SerializeField] private VR_Controller leftController = null;
        [SerializeField] private VR_CharacterController m_VRCharacterController = null;
        [SerializeField] private Camera m_centerCamera = null;
        [SerializeField] private ControllerGestureConfig gestureConfig = null;
        [SerializeField] private UnityEvent onInitializeComplete = null;
        [SerializeField] private Vector3 m_handGrabGrabRotationOffset = Vector3.zero;
      
        private VR_Integration integration = null;

        public VR_CharacterController VR_CharacterController => m_VRCharacterController;
        public Camera CenterCamera => m_centerCamera;
        
        public CharacterController CharacterController
        {
            get
            {
                return FindObjectOfType<CharacterController>();
            }
        }

        public VR_Controller LeftController
        {
            get
            {
                return GetOrInitializeController(VR_ControllerType.Left);
            }
        }

        public VR_Controller RightController
        {
            get
            {
                return GetOrInitializeController(VR_ControllerType.Right);
            }
        }

        public VR_Controller GetActiveController { get { return integration.GetActiveController(); } }
        public Transform TrackingSpace { get { return integration.GeTrackingSpaceTransform(); } }
        public Transform RealLeftHandTransform { get { return integration.GetLeftHandTransform(); } }
        public Transform RealRightHandTransform { get { return integration.GetRightHandTransform(); } }

        public Vector3 HandGrabRotationOffset => m_handGrabGrabRotationOffset;

        private VR_Controller GetOrInitializeController(VR_ControllerType controllerType)
        {
            VR_Controller controller = controllerType == VR_ControllerType.Right ? rightController : leftController;
            controller.Construct(gestureConfig);
            return controller;
        }


        protected virtual VR_Controller TryGetController(VR_ControllerType controllerType)
        {
            return controllerType == VR_ControllerType.Right ? rightController : leftController;
        }

        private Transform GetRealHandTransformForController(VR_ControllerType controllerType)
        {
            return controllerType == VR_ControllerType.Right ? RealRightHandTransform : RealLeftHandTransform;
        }

        public void Construct()
        {

            if (VR_Manager.instance.CurrentSDK == VR_SDK.Oculus)
            {
                integration = new VR_OculusIntegration();
            }

            else if (VR_Manager.instance.CurrentSDK == VR_SDK.Steam_VR)
            {
                integration = new VR_SteamVRIntegration();
            }
            else if (VR_Manager.instance.CurrentSDK == VR_SDK.UnityXR)
            {
                integration = new VR_XRIntegration();
            }
            else
            {
                Debug.LogError("[Critical Error] You have set CurrentSDK to None select VR_Manager GameObject in your current scene and please select a valid SDK");
                Debug.Break();
                return;
            }

            rightController = GetOrInitializeController(VR_ControllerType.Right);
            leftController = GetOrInitializeController(VR_ControllerType.Left);
            

            onInitializeComplete.Invoke();
        }

        private void Awake()
        {
            //notify a new player
            VR_Manager.instance.SetCurrentPlayer(this);
        }

       

    }

}

