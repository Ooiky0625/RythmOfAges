using UnityEngine;
using VRSDK;

namespace VRBeats
{
    public class VR_Hand : MonoBehaviour
    {       
        [SerializeField] private float emmisionIntensity = 100.0f;
        [SerializeField] private MaterialBindings handMaterialBindings = null;

        private void Start()
        {
            TintHand(); 
        }

        private void TintHand()
        {
            VR_ControllerType thisControllerType = GetComponent<VR_Controller>().ControllerType;
            Color handColor = VR_BeatManager.instance.GetColorFromControllerType(thisControllerType);
            handMaterialBindings.SetUseEmmisiveIntensity(false);
            handMaterialBindings.SetEmmisiveColor(handColor * emmisionIntensity);
        }


    }

}

