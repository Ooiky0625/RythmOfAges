using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageSystem
{
    public class FastCollisionListener : MonoBehaviour
    {
        [SerializeField] private BoxCollider raycastCollider = null;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private int collisionAccuracy = 5;

        private Pose[] poseArray;
        private Pose lastPose;
        private bool shouldFillPoses = false;
        private BoxCollider raycastColliderClone = null;
        private List<Collider> colliderHitThisFrame = new List<Collider>();

        private void Awake()
        {
            poseArray = new Pose[ collisionAccuracy -1 ];

            if (raycastCollider != null)
            {
                raycastColliderClone = Instantiate( raycastCollider );
                raycastColliderClone.transform.localScale = raycastCollider.transform.lossyScale;
                raycastColliderClone.isTrigger = true;
            }

        }

        private void LateUpdate()
        {
            UpdateLastPositionAndRotation();
            shouldFillPoses = true;
        }

        private void FillPoses()
        {
            if (!shouldFillPoses)
                return;

            shouldFillPoses = false;

            float step = 1.0f / collisionAccuracy;
            float currentStep = step;

            int index = 0;

            while (currentStep < 1.0f)
            {                
                Vector3 position = Vector3.Lerp(lastPose.position, raycastCollider.transform.position, currentStep);
                Quaternion rotation = Quaternion.Slerp(lastPose.rotation, raycastCollider.transform.rotation, currentStep);

                poseArray[index].position = position;
                poseArray[index].rotation = rotation;

                currentStep += step;
                index++;

            }
        }

        private void UpdateLastPositionAndRotation()
        {
            lastPose.position = raycastCollider.transform.position;
            lastPose.rotation = raycastCollider.transform.rotation;
        }

        private void PoseRaycast(Pose pose)
        {
            raycastColliderClone.transform.position = pose.position;
            raycastColliderClone.transform.rotation = pose.rotation;
            Collider[] colliderArray = PhysicsExtensions.OverlapBox(raycastColliderClone, layerMask, QueryTriggerInteraction.Ignore);

            for (int n = 0; n < colliderArray.Length; n++)
            {
                if (colliderArray[n] == null) continue;
                
                if (!colliderHitThisFrame.Contains(colliderArray[n]))
                {                    
                    colliderHitThisFrame.Add(colliderArray[n]);
                }

            }
        }

        public List<Collider> CheckForCollisionsThisFrame()
        {
            colliderHitThisFrame = new List<Collider>();
            FillPoses();

            for (int n = 0; n < poseArray.Length; n++)
            {
                PoseRaycast( poseArray[n] );
            }

            return colliderHitThisFrame;
        }

    }

    public struct Pose
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}

