using UnityEngine;
using System;

namespace VRSDK
{
    public class OnJointBreakListener : MonoBehaviour
    {
        private Action<float> onJointBreakCallback = null;

        public void SetListener(Action<float> listener)
        {
            onJointBreakCallback = listener;
        }

        public void RemoveAllListeners()
        {
            onJointBreakCallback = null;
        }

        //method called by Unity when a joint gets break in the gameobject
        private void OnJointBreak(float f)
        {
            if (onJointBreakCallback != null)
                onJointBreakCallback( f );
        }
    }

}

