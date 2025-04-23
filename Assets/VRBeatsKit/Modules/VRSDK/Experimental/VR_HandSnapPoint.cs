namespace VRSDK
{
    public class VR_HandSnapPoint : VR_Grabbable
    {

        protected override void Start()
        {
            base.Start();
            preventDefault = true;
        }

       
        private void LateUpdate()
        {
            if (currentGrabState != GrabState.Grab)
                return;

            activeController.transform.position = GetCurrentHandInteractSettings().interactPoint.position;
        }

        public override void OnGrabSuccess(VR_Controller controller)
        {
            activeController = controller;
            currentGrabState = GrabState.Grab;
            RaiseOnGrabStateChangeEvent( GrabState.Grab );

            GrabController.SetVisibility( !GetCurrentHandAnimationSettings().hideHandOnGrab );

            activeController.SetPositionControlMode( MotionControlMode.Free );
            activeController.SetRotationControlMode( MotionControlMode.Free );
                      
        }
    
    }
}

