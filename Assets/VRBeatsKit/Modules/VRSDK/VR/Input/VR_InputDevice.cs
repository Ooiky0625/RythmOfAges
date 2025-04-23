using UnityEngine;

namespace VRSDK
{
    /// <summary>
    /// Supported Inputs
    /// </summary>
    public enum VR_InputButton
    {
        None,
        Primary,
        Secondary,
        Trigger,
        Grip,
        TumbstickPress    
    }

    public abstract class VR_InputDevice
    {
        protected VR_Controller InputController { get; private set; }

        public VR_InputDevice(VR_Controller controller)
        {
            this.InputController = controller;
        }

        public abstract Quaternion GetRotationOffset();
        public abstract Vector3 GetPositionOffset();
        public abstract string GetDeviceName();                
        public abstract float GetAxis1D(VR_InputButton button);
        public abstract Vector2 GetJoystick();
        public abstract bool GetButtonDown(VR_InputButton button);
        public abstract bool GetButtonUp(VR_InputButton button);
        public abstract bool GetButton(VR_InputButton button);        
        public abstract bool IsConnected();
        public abstract System.Enum GetControllerType();
    }
}
