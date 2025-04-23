using UnityEngine;
using System.Collections;

namespace VRSDK
{
    public class VR_GrabCloner : MonoBehaviour
    {
        #region INSPECTOR
        [SerializeField] private VR_Grabbable grabbablePrefab = null;
        #endregion

        #region PRIVATE
        private VR_Grabbable currentGrab = null;
        #endregion

        private void Awake()
        {
            currentGrab = Instantiate( grabbablePrefab, transform.position, transform.rotation );
            currentGrab.OnGrabStateChange.AddListener( OnGrabStateChange );
        }

        private void OnGrabStateChange(GrabState grabState)
        {
            currentGrab.OnGrabStateChange.RemoveListener( OnGrabStateChange );
            currentGrab = Instantiate( grabbablePrefab, currentGrab.transform.position, currentGrab.transform.rotation );

            currentGrab.OnGrabStateChange.AddListener( OnGrabStateChange );
        }

    }
}

