using System;
using UnityEngine;

namespace VRSDK
{
    public class FaceCamera : MonoBehaviour
    {
        [SerializeField] private float multiplier = 1.0f;
        
        private Transform mainCamera = null;
        private Transform thisTransform = null;

        private void Awake()
        {
            thisTransform = transform;
        }

        private void LateUpdate()
        {
            if (mainCamera == null)
            {
                mainCamera = GetMainCamera();
            }

            if (mainCamera == null)
            {
                return;
            }

            thisTransform.forward = (mainCamera.position - transform.position).normalized * multiplier;
        }

        private Transform GetMainCamera()
        {
            Camera cam = Camera.main;

            if (cam != null)
            {
                return cam.transform;
            }

            return null;
        }

    }

}

