using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourdplan
{
    public class FaceCamera : MonoBehaviour
    {
        [SerializeField] private int direction = 1;
        [SerializeField] private Transform overrideLookAt = null;

        public Transform LookAt { get { return overrideLookAt == null ? Camera.main.transform : overrideLookAt; } }

        private void LateUpdate()
        {
            Vector3 dir = LookAt.forward;
            transform.forward = (LookAt.position - transform.position).normalized * direction;
        }
    }

}

