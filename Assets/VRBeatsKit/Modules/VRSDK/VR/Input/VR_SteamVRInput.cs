using System;
using UnityEngine;
#if SDK_STEAM_VR
using Valve.VR;
#endif

namespace VRSDK 
{
    public class VR_SteamVRInput : VR_InputDevice
    {
        #if SDK_STEAM_VR
        private SteamVR_Action_Single grabAction = null;
        private SteamVR_Action_Boolean triggerAction = null;
        private SteamVR_Action_Boolean primaryButtonAction = null;
        private SteamVR_Action_Boolean secondaryButtonAction = null;
        private SteamVR_Action_Boolean joystickPressAction = null;
        private SteamVR_Action_Vector2 joystickInputAction = null;
       

        public SteamVR_Input_Sources SteamControllerType { get { return InputController.ControllerType == VR_ControllerType.Right ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand; } }
       
        private bool isConnected = false;
       #endif

        public VR_SteamVRInput(VR_Controller controller) : base(controller) 
        {
            #if SDK_STEAM_VR
            InitializeSteamVR_Actions();

            SteamVR_Behaviour_Pose steamController = controller.GetComponentInParent<SteamVR_Behaviour_Pose>();
            steamController.onConnectedChanged.AddListener(delegate (SteamVR_Behaviour_Pose poseController, SteamVR_Input_Sources sources, bool state) { isConnected = state; });
            #endif
        }

        #if SDK_STEAM_VR
        private void InitializeSteamVR_Actions()
        {
            grabAction = SteamVR_Input.GetAction<SteamVR_Action_Single>("VRShooterKit", "Grab");
            triggerAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("VRShooterKit", "Shoot");
            primaryButtonAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("VRShooterKit", "PrimaryButton");
            secondaryButtonAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("VRShooterKit", "SecondaryButton");
            joystickPressAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("VRShooterKit", "JoystickPress");
            joystickInputAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("VRShooterKit", "Joystick");
        }
        #endif

        public override float GetAxis1D(VR_InputButton button)
        {
            #if SDK_STEAM_VR
            switch (button)
            {

                case VR_InputButton.Trigger:
                    return GetButton(button) ? 1.0f : 0.0f;                    

                case VR_InputButton.Grip:
                    return grabAction.GetAxis(SteamControllerType);
            }
            #endif

            return 0.0f;
        }

        public override bool GetButton(VR_InputButton button)
        {
            #if SDK_STEAM_VR
            switch (button)
            {
                case VR_InputButton.Trigger:
                    return triggerAction.GetState(SteamControllerType);                   
                    
                case VR_InputButton.Grip:
                    return grabAction.GetAxis(SteamControllerType) > 0.65f;             

                case VR_InputButton.Primary:
                    return primaryButtonAction.GetState(SteamControllerType);

                case VR_InputButton.Secondary:
                    return secondaryButtonAction.GetState(SteamControllerType);
                case VR_InputButton.TumbstickPress:
                    return joystickPressAction.GetState(SteamControllerType);
            }
            #endif
            return false;
        }

        public override bool GetButtonDown(VR_InputButton button)
        {
            #if SDK_STEAM_VR
            switch (button)
            {
                case VR_InputButton.Trigger:
                    return triggerAction.GetStateDown(SteamControllerType);

                case VR_InputButton.Grip:
                    return grabAction.GetAxis(SteamControllerType) > 0.65f;  

                case VR_InputButton.Primary:
                    return primaryButtonAction.GetStateDown(SteamControllerType);

                case VR_InputButton.Secondary:
                    return secondaryButtonAction.GetStateDown(SteamControllerType);
                case VR_InputButton.TumbstickPress:
                    return joystickPressAction.GetStateDown(SteamControllerType);
            }
            #endif
            return false;
        }

        public override bool GetButtonUp(VR_InputButton button)
        {
            #if SDK_STEAM_VR
            switch (button)
            {
                case VR_InputButton.Trigger:
                    return triggerAction.GetStateUp(SteamControllerType);

                case VR_InputButton.Grip:
                    return grabAction.GetAxis(SteamControllerType) > 0.65f; 

                case VR_InputButton.Primary:
                    return primaryButtonAction.GetStateUp(SteamControllerType);

                case VR_InputButton.Secondary:
                    return secondaryButtonAction.GetStateUp(SteamControllerType);
                case VR_InputButton.TumbstickPress:
                    return joystickPressAction.GetStateUp(SteamControllerType);
            }
            #endif

            return false;
        }

        public override string GetDeviceName()
        {
            #if SDK_STEAM_VR
            return SteamControllerType.ToString();
            #endif

            return "";
        }

        public override Vector2 GetJoystick()
        {
            #if SDK_STEAM_VR
            return joystickInputAction.GetAxis(SteamControllerType);
            #endif

            return Vector2.zero;
        }

        public override bool IsConnected()
        {
            #if SDK_STEAM_VR
            return isConnected;
            #endif

            return false;
        }

        public override Quaternion GetRotationOffset()
        {
            return Quaternion.identity;
        }

        public override Vector3 GetPositionOffset()
        {
            return Vector3.zero;
        }

        public override Enum GetControllerType()
        {
            #if SDK_STEAM_VR
            return SteamControllerType;
            #endif

            return default;
        }
    }
}
