using UnityEngine;
using System.Collections;

namespace VRSDK
{
    //this script is being use for the arrow grab zone when the hand is on range and you press the interact
    //button it creates a the grabbable in your hand
    public class VR_GrabbableZone : VR_Interactable
    {
        [SerializeField] private VR_Grabbable grabbable = null;

        public VR_Grabbable Grabbable { get { return grabbable; } }

        protected override void Awake()
        {
            shareHandInteractionSettings = true;
            handSettings.canInteract = true;
            handSettings.interactPoint = transform;
            handSettings.highlightPoint = transform;

            base.Awake();

           
        }

        public override void Interact(VR_Controller controller)
        {
            Debug.Log("interact");
            StartCoroutine( GrabRoutine(controller) );
        }   

        private IEnumerator GrabRoutine(VR_Controller controller)
        {
            VR_Grabbable clone = Instantiate( grabbable, transform.position, Quaternion.identity );
            controller.ForceGrab( clone );

            MeshRenderer[] renderArray = clone.transform.GetComponentsInChildren<MeshRenderer>();

            for (int n = 0; n < renderArray.Length; n++)
            {
                renderArray[n].enabled = false;
            }

            yield return new WaitForEndOfFrame();

            for (int n = 0; n < renderArray.Length; n++)
            {
                renderArray[n].enabled = true;
            }

            clone.gameObject.SetActive( true );
        }
        
        
    }

}

