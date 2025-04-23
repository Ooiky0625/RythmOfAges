using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSDK
{
    public class OverrideRigidBodyCOM : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb = null;
        [SerializeField] private Transform com = null;

        private void Awake()
        {
            rb.centerOfMass = com.transform.localPosition;
        }
    }

}
