using UnityEngine;

namespace VRSDK
{
    public class ParentOnDrop : MonoBehaviour
    {
        [SerializeField] private Transform parent = null;

        private void Awake()
        {
            VR_DropZone dropzone = GetComponent<VR_DropZone>();

            if (dropzone != null)
            {
                dropzone.OnDrop.AddListener( OnDropStateChange );
            }
            
        }

        private void OnDropStateChange(VR_Grabbable grabbable)
        {
            if(grabbable != null)
                grabbable.transform.parent = parent;
        }
    }

}

