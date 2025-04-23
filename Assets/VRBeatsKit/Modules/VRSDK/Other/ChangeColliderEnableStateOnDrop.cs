using UnityEngine;

namespace VRSDK
{
    public class ChangeColliderEnableStateOnDrop : MonoBehaviour
    {
        [SerializeField] private bool onDropColliderState = false;
        [SerializeField] private bool onGrabColliderState = false;

        private VR_DropZone dropZone = null;
       
        private void Awake()
        {
            dropZone = GetComponent<VR_DropZone>();
          
            dropZone.OnDrop.AddListener( OnDropStateChange );
        }

        private void OnDropStateChange(VR_Grabbable grabbable)
        {          
            SetColliderState( grabbable , onDropColliderState );
        }

        private void OnUnDropStateChange(VR_Grabbable grabbable)
        {
            SetColliderState( grabbable , onGrabColliderState );
        }

        private void SetColliderState(VR_Grabbable grabbble , bool state)
        {
            Collider col = grabbble.GetComponent<Collider>();

            if (col != null)
                col.enabled = state;
        }

    }
}

