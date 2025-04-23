using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRSDK
{
    public class VR_AutoDrop : MonoBehaviour
    {
        [SerializeField] private VR_Grabbable grabbable = null;
        [SerializeField] private VR_DropZone dropzone = null;

        private void Start()
        {
            grabbable.OnGrabStateChange.AddListener(OnGrabStateChange);
        }

        private void OnGrabStateChange(GrabState state)
        {
            if (state == GrabState.UnGrab)
            {
                dropzone.OnGrabStateChange(GrabState.Drop, grabbable, true);
            }
        }

    }
}

