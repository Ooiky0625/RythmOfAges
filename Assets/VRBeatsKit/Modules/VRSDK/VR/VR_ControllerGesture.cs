using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using VRSDK.Events;

namespace VRSDK
{
    public enum GesturePhase
    {
        Tracking,
        Processing
    }

    //this script read gestures in the controller
    //like a rotation a certain speed and angles
    //this is being use for the weapon system for physics base reload, is beinguse in the revolver
    public class VR_ControllerGesture : MonoBehaviour
    {
        private float minAcelerationThreshold = 0.0f;
        private float maxAcelerationThreshold = 0.0f;
        private int sampleCount = 0;

        private VR_Controller controller = null;
        private GesturePhase rotationGesturePhase = GesturePhase.Tracking;       
        private Quaternion rotationGesturefromQuaternion = Quaternion.identity;
        private OnRotationGestureEvent OnRotationGestureEvent = new OnRotationGestureEvent();

        private const int MAX_SAMPLING_HISTORY = 30;

        private void Awake()
        {
            controller = GetComponent<VR_Controller>();
        }

        public void Construct(ControllerGestureConfig config)
        {            
            minAcelerationThreshold = config.minAcelerationThreshold;
            maxAcelerationThreshold = config.maxAcelerationThreshold;            
        }

        private void Update()
        {
            UpdateRotationGesture();           
        }

        // set a listener for a rotation gesture
        public void ListenForRotationGesture(UnityAction<RotationGestureInfo> listener)
        {
            OnRotationGestureEvent.AddListener(listener);
        }

        //remove listener for a rotation gesture
        public void RemoveRotationGestureListener(UnityAction<RotationGestureInfo> listener)
        {
            OnRotationGestureEvent.RemoveListener(listener);
        }

        private void UpdateRotationGesture()
        {
            switch (rotationGesturePhase)
            {
                case GesturePhase.Tracking:
                    RotationGestureTrackingUpdate();
                    break;
                case GesturePhase.Processing:
                    RotationGestureProcessingUpdate();
                    break;
                
            }
        }

        private void RotationGestureTrackingUpdate()
        {
            List<Quaternion> rotationHistory = GetRotationHistory();

            if (rotationHistory == null || rotationHistory.Count <= 0)
                return;

            float rotAceleration = Quaternion.Angle( rotationHistory[0], rotationHistory[rotationHistory.Count - 1] );

            //detect a fast rotation movement
            if (rotAceleration > maxAcelerationThreshold)
            {
                rotationGesturePhase = GesturePhase.Processing;
            }

        }

        private List<Quaternion> GetRotationHistory()
        {
            List<Quaternion> rotationHistory = controller.GetRotationHistorySample( MAX_SAMPLING_HISTORY );
            
            return rotationHistory;
        }

        private void RotationGestureProcessingUpdate()
        {
            List<Quaternion> rotationHistory = GetRotationHistory();

            if (rotationHistory == null || rotationHistory.Count <= 0)
                return;

            float rotAceleration = Quaternion.Angle( rotationHistory[0], rotationHistory[rotationHistory.Count - 1] );

            //wait for the hand to stop
            if (rotAceleration < minAcelerationThreshold)
            {
                OnRotationGestureEvent.Invoke( new RotationGestureInfo( rotationGesturefromQuaternion , rotationHistory[rotationHistory.Count - 1] ) );
                rotationGesturePhase = GesturePhase.Tracking;
            }

        }      

    }

    public class RotationGestureInfo
    {
        public Quaternion from;
        public Quaternion to;

        public RotationGestureInfo(Quaternion from , Quaternion to)
        {
            this.from = from;
            this.to = to;
        }
        

    }

}

