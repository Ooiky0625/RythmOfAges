using UnityEngine;
using System.Collections.Generic;
using VRSDK.Collections;

namespace VRSDK 
{
    public class XRInputHelper : MonoBehaviour
    {
        private List<VR_InputButton> trackingButtonList = null;
        private Dictionary<VR_InputButton, Buffer<bool>> buttonStateHistory = new Dictionary<VR_InputButton, Buffer<bool>>();
        private int historySize = 3;
        private VR_InputDevice input = null;

        public void Construct(VR_InputDevice input , List<VR_InputButton> trackingButtonList)
        {
            this.input = input;
            this.trackingButtonList = trackingButtonList;
            SetupButtonInfo();
        }

        private void SetupButtonInfo()
        {
            for (int n = 0; n < trackingButtonList.Count; n++)
            {
                buttonStateHistory.Add( trackingButtonList[n] , new Buffer<bool>(historySize) );               
            }
        }

        private void Update()
        {
            for (int n = 0; n < trackingButtonList.Count; n++)
            {
                Buffer<bool> buffer;
                if ( buttonStateHistory.TryGetValue( trackingButtonList[n] , out buffer ) )
                {
                    buffer.Add( input.GetButton(trackingButtonList[n]) );
                }
            }
        }

        public bool GetButtonDown(VR_InputButton button)
        {
            Buffer<bool> buffer;
            if (buttonStateHistory.TryGetValue(button, out buffer))
            {
                if (buffer.Count >= historySize && !buffer[historySize - 2] && input.GetButton(button))
                    return true;
            }

            return false;
        }

        public bool GetButtonUp(VR_InputButton button)
        {
            Buffer<bool> buffer;
            if (buttonStateHistory.TryGetValue(button, out buffer))
            {
                if (buffer.Count >= historySize && buffer[historySize - 2] && !input.GetButton(button))
                    return true;
            }

            return false;
        }



    }

    public struct XRButtonInfo
    {
        public VR_InputButton button;
        public bool state;       

    }

}
