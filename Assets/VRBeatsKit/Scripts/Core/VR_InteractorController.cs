using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRBeats
{
    public class VR_InteractorController : MonoBehaviour
    {
        private XRRayInteractor rayInteractor = null;
        private XRInteractorLineVisual interactorLineVisual = null;
        private LineRenderer lineRender = null;
        
        private void Awake()
        {
            rayInteractor = GetComponent<XRRayInteractor>();
            interactorLineVisual = GetComponent<XRInteractorLineVisual>();
            lineRender = GetComponent<LineRenderer>();
                        
            
            DisableXRRayInteractorComponents();
        }
        
        public void DisableXRRayInteractorComponents()
        {
            rayInteractor.enabled = false;
            interactorLineVisual.enabled = false;
            lineRender.enabled = false;
        }

        public void EnableXRRayInteractorComponents()
        {           
            rayInteractor.enabled = true;
            interactorLineVisual.enabled = true;
            lineRender.enabled = true;
        }

    }

}
