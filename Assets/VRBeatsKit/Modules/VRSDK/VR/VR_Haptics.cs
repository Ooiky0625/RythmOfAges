using UnityEngine;

#if SDK_STEAM_VR
using Valve.VR;
#endif

namespace VRSDK
{
    public class VR_Haptics : MonoBehaviour
    {

        [SerializeField] private float rate = 0.1f;
        [SerializeField] private float multiplier = 1.0f;

        private float lastHapticsFeedback = 0.0f;
        private bool givingFeedback = false;
        private VR_Controller activeController = null;
        private VR_Grabbable grabbable = null;
#if SDK_STEAM_VR
        private SteamVR_Action_Vibration hapticAction = null;
#endif

        private const float MIN_HAPTICS_VALUE = 0.2f;

        private void Awake()
        {
#if SDK_STEAM_VR
            hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>( "VRShooterKit", "Haptic" );
#endif
            grabbable = GetComponent<VR_Grabbable>();
            grabbable.OnGrabStateChange.AddListener( OnGrabStateChange );
        }

        private void OnGrabStateChange(GrabState state)
        {
            if (state == GrabState.Grab)
            {
                activeController = grabbable.GrabController;
            }
        }


        private void Update()
        {
#if SDK_OCULUS
            if (givingFeedback && Time.time - lastHapticsFeedback > rate)
                Stop();
#endif
        }

        public void SetHaptics(float frequency, float amplitud , VR_Controller controller)
        {
            frequency *= multiplier;
            amplitud *= multiplier;

            if (amplitud < MIN_HAPTICS_VALUE)
                amplitud = MIN_HAPTICS_VALUE;

            if (Time.time - lastHapticsFeedback  < rate)
                return;

            if (!givingFeedback)
                givingFeedback = true;

            lastHapticsFeedback = Time.time;
#if SDK_OCULUS            
            if(activeController == null) return;
            OVRInput.SetControllerVibration(frequency , amplitud , (OVRInput.Controller) activeController.Input.GetControllerType() );
#endif
#if SDK_STEAM_VR           
            if(controller == null) return;
            hapticAction.Execute( 0.0f, rate, Mathf.Min( frequency * 320.0f, 320.0f ), amplitud, (SteamVR_Input_Sources) controller.Input.GetControllerType() );
#endif
            activeController = controller;
        }

        public void SetHaptics(float value , VR_Controller controller)
        {
            value *= multiplier;

            SetHaptics(Random.Range(value / 2.0f , value) , value , controller);
        }

        public void Stop()
        {
            givingFeedback = false;
            
            if(activeController == null) return;
#if SDK_OCULUS           
            OVRInput.SetControllerVibration( 0.0f, 0.0f, (OVRInput.Controller)activeController.Input.GetControllerType());
#endif
#if SDK_STEAM_VR
            hapticAction.Execute(0.0f , 0.01f , 0.0f , 0.0f , (SteamVR_Input_Sources) activeController.Input.GetControllerType());
#endif
        }

    }

}

