using UnityEngine;
using VRSDK.Events;

namespace VRSDK
{
    public class VR_Slider : VR_Grabbable
    {
        [SerializeField] private Axis slideAxis = Axis.Forward;        
        [SerializeField] private Transform slideStartMarker = null;
        [SerializeField] private Transform slideEndMarker = null;
        [SerializeField] private OnValueChangeEvent onValueChange = null;

        private Vector3 initialLocalPosition = Vector3.zero;
        private Vector3 slideStartMarkeLocalPosition = Vector3.zero;
        private Vector3 slideEndMarkerLocalPosition = Vector3.zero;
        private Vector3 calculateControllerLocalPosition = Vector3.zero;             
        private float movementRange = 0.0f;
        private float currentValue = 0.0f;

        public OnValueChangeEvent OnValueChange { get { return onValueChange; } }

        protected override void Awake()
        {
            base.Awake();

            initialLocalPosition = transform.localPosition;
            slideStartMarkeLocalPosition = slideStartMarker.localPosition;
            slideEndMarkerLocalPosition = slideEndMarker.localPosition;

            movementRange = CalculateMovementRange();
        }

       

        private void LateUpdate()
        {
            if (activeController == null)
                return;

            Vector3 controllerPosition = activeController.OriginalParent.position;
            calculateControllerLocalPosition = transform.parent.InverseTransformPoint( activeController.OriginalParent.position );            
        }
              
        protected override void GrabUpdate ()
        {
            if (activeController == null)
                return;
           
            
            if (CanSlide(calculateControllerLocalPosition))
            {
                if (slideAxis == Axis.Horizontal)
                {

                    transform.localPosition = new Vector3( calculateControllerLocalPosition.x, initialLocalPosition.y, initialLocalPosition.z );
                }
                else if (slideAxis == Axis.Vertical)
                {
                    transform.localPosition = new Vector3( initialLocalPosition.x, calculateControllerLocalPosition.y, initialLocalPosition.z );
                }
                else if (slideAxis == Axis.Forward)
                {
                    transform.localPosition = new Vector3( initialLocalPosition.x, initialLocalPosition.y, calculateControllerLocalPosition.z );
                }

                
            }    

          
            UpdateSlideValue(calculateControllerLocalPosition);

            base.GrabUpdate();
        }
        

        private bool CanSlide(Vector3 controllerLocalPosition)
        {
            if (slideAxis == Axis.Horizontal)
            {                
                return slideStartMarkeLocalPosition.x < controllerLocalPosition.x && slideEndMarkerLocalPosition.x > controllerLocalPosition.x;
            }

            if (slideAxis == Axis.Vertical)
            {
                return slideStartMarkeLocalPosition.y < controllerLocalPosition.y && slideEndMarkerLocalPosition.y > controllerLocalPosition.y;
            }

            if (slideAxis == Axis.Forward)
            {              

                return slideStartMarkeLocalPosition.z < controllerLocalPosition.z && slideEndMarkerLocalPosition.z > controllerLocalPosition.z;
            }

            return false;
        }

        private void UpdateSlideValue(Vector3 controllerLocalPosition)
        {
            float distance = CalculateDistance( controllerLocalPosition );
            float value = Mathf.Clamp01( distance / movementRange );

            if (Mathf.Abs( value - currentValue ) > 0.01)
            {
                onValueChange.Invoke( value );
                currentValue = value;
            }

        }


        private float CalculateDistance(Vector3 controllerLocalPosition)
        {

            if (ControllerIsBeyondRange( controllerLocalPosition ))
                return 1.0f;
            else if (ControllerIsBelowRange( controllerLocalPosition ))
                return 0.0f;

                if (slideAxis == Axis.Horizontal)
            {               

                return Mathf.Abs( slideStartMarkeLocalPosition.x - controllerLocalPosition.x );
            }

            if (slideAxis == Axis.Vertical)
            {
                return Mathf.Abs( slideStartMarkeLocalPosition.y - controllerLocalPosition.y );
            }

            if (slideAxis == Axis.Forward)
            {
                return Mathf.Abs( slideStartMarkeLocalPosition.z - controllerLocalPosition.z );
            }

            return 0.0f;
        }
        
        private float CalculateMovementRange()
        {
            if (slideAxis == Axis.Horizontal)
            {
                return Mathf.Abs( slideStartMarkeLocalPosition.x - slideEndMarkerLocalPosition.x );
            }

            if (slideAxis == Axis.Vertical)
            {
                return Mathf.Abs( slideStartMarkeLocalPosition.y - slideEndMarkerLocalPosition.y );
            }

            if (slideAxis == Axis.Forward)
            {
                return Mathf.Abs( slideStartMarkeLocalPosition.z - slideEndMarkerLocalPosition.z );
            }

            return 0.0f;
        }

       
        private bool ControllerIsBeyondRange(Vector3 controllerLocalPosition)
        {
            if (slideAxis == Axis.Horizontal)
            {
                return slideEndMarkerLocalPosition.x < controllerLocalPosition.x;
            }

            if (slideAxis == Axis.Vertical)
            {
                return slideEndMarkerLocalPosition.y < controllerLocalPosition.y;
            }

            if (slideAxis == Axis.Forward)
            {
                return slideEndMarkerLocalPosition.z < controllerLocalPosition.z;
            }

            return false;
        }

        private bool ControllerIsBelowRange(Vector3 controllerLocalPosition)
        {
            if (slideAxis == Axis.Horizontal)
            {
                return slideStartMarkeLocalPosition.x > controllerLocalPosition.x;
            }

            if (slideAxis == Axis.Vertical)
            {
                return slideStartMarkeLocalPosition.y > controllerLocalPosition.y;
            }

            if (slideAxis == Axis.Forward)
            {
                return slideStartMarkeLocalPosition.z > controllerLocalPosition.z;
            }

            return false;
        }

        public override void OnGrabSuccess(VR_Controller controller)
        {
            activeController = controller;
            currentGrabState = GrabState.Grab;
            RaiseOnGrabStateChangeEvent( GrabState.Grab );

            GrabController.SetVisibility( !GetCurrentHandAnimationSettings().hideHandOnGrab );
        }

      
    }

}

