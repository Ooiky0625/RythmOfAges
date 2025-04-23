using UnityEngine;
using System.Collections;

namespace VRSDK
{   
    public class VR_Trowable : MonoBehaviour
    {
        [SerializeField] private float speedModifier = 1.5f;
        [SerializeField] private float aungularSpeedModifier = 1.5f;
        [SerializeField] private float maxAngularVelocity = 10.0f;

        private Rigidbody rb = null;

        public float SpeedModifier { get { return speedModifier; } }
        public float AngularSpeedModifier { get { return aungularSpeedModifier; } }
        bool throwed = false;
        private void Awake()
        {
            
            rb = GetComponent<Rigidbody>();
            rb.maxAngularVelocity = maxAngularVelocity;

            VR_Grabbable grabbable = GetComponent<VR_Grabbable>();

            if (grabbable == null)
            {
                Debug.LogError( "Trowable needs VR_Grabbable script in order to work!" );
                return;
            }
            
            GetComponent<VR_Grabbable>().OnAfterThrow.AddListener( delegate 
            {                
                rb.velocity *= speedModifier;
                rb.angularVelocity *= aungularSpeedModifier;                
            } );
            
        }

    }

}

