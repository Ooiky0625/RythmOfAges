using System;
using UnityEngine;

namespace VRSDK
{
    public class VR_OculusInput : VR_InputDevice
    {
        #if SDK_OCULUS
        private OVRInput.Controller OVRControllerType
        {
            get 
            {
                
                return InputController.ControllerType == VR_ControllerType.Right ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch;

            }
        }
        #endif

        public VR_OculusInput(VR_Controller controller) : base(controller) { }

        public override float GetAxis1D(VR_InputButton button)
        {
            #if SDK_OCULUS
            OVRInput.Axis1D axis;

            switch (button)
            {

                case VR_InputButton.Trigger:
                   
                    axis = OVRInput.Axis1D.PrimaryIndexTrigger;
                    return OVRInput.Get(axis, OVRControllerType);

                case VR_InputButton.Grip:
                    
                    axis = OVRInput.Axis1D.PrimaryHandTrigger;
                    return OVRInput.Get(axis, OVRControllerType);                 

            }
            #endif  

            return 0.0f;
        }

        public override bool GetButton(VR_InputButton button)
        {
#if SDK_OCULUS
            switch (button)
            {
                case VR_InputButton.Grip:
                    return GetAxis1D( button ) > 0.25f;
                case VR_InputButton.Trigger:
                    return GetAxis1D( button ) > 0.25f;
                case VR_InputButton.Primary:
                    return OVRInput.Get(OVRInput.Button.One, OVRControllerType);
                case VR_InputButton.Secondary:
                    return OVRInput.Get(OVRInput.Button.Two, OVRControllerType);
                case VR_InputButton.TumbstickPress:
                    return OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRControllerType);
            }
#endif
            return false;
           
        }

        public override bool GetButtonDown(VR_InputButton button)
        {
#if SDK_OCULUS
            switch (button)
            {
                case VR_InputButton.Grip:
                    return OVRInput.GetDown( OVRInput.Button.PrimaryHandTrigger , OVRControllerType );
                case VR_InputButton.Trigger:
                    return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRControllerType);
                case VR_InputButton.Primary:
                    return OVRInput.GetDown(OVRInput.Button.One, OVRControllerType);
                case VR_InputButton.Secondary:
                    return OVRInput.GetDown(OVRInput.Button.Two, OVRControllerType);
                case VR_InputButton.TumbstickPress:
                    return OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRControllerType);
            }
#endif
            return false;
           
        }

        public override bool GetButtonUp(VR_InputButton button)
        {
            #if SDK_OCULUS
            switch (button)
            {
                case VR_InputButton.Grip:
                    return OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRControllerType);
                case VR_InputButton.Trigger:
                    return OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRControllerType);
                case VR_InputButton.Primary:
                    return OVRInput.GetUp(OVRInput.Button.One, OVRControllerType);
                case VR_InputButton.Secondary:
                    return OVRInput.GetUp(OVRInput.Button.Two, OVRControllerType);
                case VR_InputButton.TumbstickPress:
                    return OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRControllerType);
          
            }
            #endif

            return false;
        }

        public override string GetDeviceName()
        {
            #if SDK_OCULUS
            return OVRControllerType.ToString();
            #endif
            return "";
        }

        public override Vector2 GetJoystick()
        {
            #if SDK_OCULUS
            OVRInput.Axis2D axis = OVRInput.Axis2D.PrimaryThumbstick;
            return OVRInput.Get(axis, OVRControllerType);
            #endif
            return Vector2.zero;
        }

        public override bool IsConnected()
        {
            #if SDK_OCULUS
            return OVRInput.IsControllerConnected(OVRControllerType);
            #endif
            return false;
        }

        public override Quaternion GetRotationOffset()
        {
            #if SDK_OCULUS
            return Quaternion.identity;
            #endif

            return Quaternion.identity;
        }

        public override Vector3 GetPositionOffset()
        {
            return Vector3.zero;
        }

        public override Enum GetControllerType()
        {
#if SDK_OCULUS
            return OVRControllerType;
#endif
            return default;
        }
    }

}

