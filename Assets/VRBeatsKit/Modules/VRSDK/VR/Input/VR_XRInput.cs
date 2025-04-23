using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_XR
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
#endif

namespace VRSDK
{
    public class VR_XRInput : VR_InputDevice
    {
        
        private InputDevice thisInputDevice;
        private XRInputHelper inputHelper = null;
        private List<VR_InputButton> inputButtonTrackedList = new List<VR_InputButton>() 
        { 
            VR_InputButton.Grip , 
            VR_InputButton.Primary , 
            VR_InputButton.Secondary , 
            VR_InputButton.Trigger , 
            VR_InputButton.TumbstickPress 
        };

        public VR_XRInput(VR_Controller controller) : base(controller)
        {
            GameObject go = new GameObject( "XRInputHelper" );
            inputHelper = go.AddComponent<XRInputHelper>();
            inputHelper.Construct(this , inputButtonTrackedList);

            InputDevices.deviceConnected += RegisterDevice;
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);
            for (int i = 0; i < devices.Count; i++)
                RegisterDevice(devices[i]);
        }

        private void RegisterDevice(InputDevice connectedDevice)
        {            
            if (IsValidInputDeviceForController(InputController, connectedDevice))
            {
                thisInputDevice = connectedDevice;
            }
        }

        private bool IsValidInputDeviceForController(VR_Controller controller , InputDevice inputDevice)
        {
            if (controller.ControllerType == VR_ControllerType.Right)
            {
                return InputDeviceIsRightController(inputDevice);
            }
            else
            {
                return InputDeviceIsLeftController(inputDevice);
            }            
        }

        private bool InputDeviceIsRightController(InputDevice inputDevice)
        {            
            return (inputDevice.characteristics & InputDeviceCharacteristics.Right) != 0;
        }

        private bool InputDeviceIsLeftController(InputDevice inputDevice)
        {
            return (inputDevice.characteristics & InputDeviceCharacteristics.Left) != 0;
        }

        public override float GetAxis1D(VR_InputButton button)
        {            
            float value = 0.0f;

            switch (button)
            {
                case VR_InputButton.Trigger:
                    thisInputDevice.TryGetFeatureValue(CommonUsages.trigger, out value);
                    break;
                case VR_InputButton.Grip:
                    thisInputDevice.TryGetFeatureValue(CommonUsages.grip, out value);
                    break;
            }

            return value;
        }

        public override bool GetButton(VR_InputButton button)
        {
            bool value = false;
            
            switch (button)
            {
                case VR_InputButton.Grip:
                    return GetAxis1D(button) > 0.25f;
                case VR_InputButton.Trigger:
                    return GetAxis1D(button) > 0.25f;
                case VR_InputButton.Primary:
#if UNITY_XR
                    thisInputDevice.IsPressed(InputHelpers.Button.PrimaryButton, out value);
#endif
                    break;
                case VR_InputButton.Secondary:
#if UNITY_XR
                    thisInputDevice.IsPressed(InputHelpers.Button.SecondaryButton, out value);
#endif
                    break;
                case VR_InputButton.TumbstickPress:
#if UNITY_XR
                    thisInputDevice.IsPressed(InputHelpers.Button.Primary2DAxisClick, out value);
#endif
                    break;
            }

            return value;
        }

        public override bool GetButtonDown(VR_InputButton button)
        {
            return inputHelper.GetButtonDown(button);
        }

        public override bool GetButtonUp(VR_InputButton button)
        {
            return inputHelper.GetButtonUp(button);
        }

        public override string GetDeviceName()
        {
            return thisInputDevice.name;
        }

        public override Vector2 GetJoystick()
        {
            Vector2 joystick;
            thisInputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis,  out joystick);

            return joystick;
        }

        public override Vector3 GetPositionOffset()
        {
            return Vector3.zero;
        }

        public override Quaternion GetRotationOffset()
        {
            return Quaternion.identity;
        }

        public override bool IsConnected()
        {
            return true;
        }

        public override Enum GetControllerType()
        {
            return default;
        }
    }

}

