using System.Collections.Generic;
using UnityEngine;

namespace VRSDK.Locomotion
{
    public class VR_CharacterAimRaycaster : VR_AimRaycaster
    {
        [SerializeField] private float characterRadiusOffset = 1.5f;
        [SerializeField] private CharacterController characterController = null;
        [SerializeField] private float slopeLimit = 20.0f;

        protected override AimRaycastInfo ProcessHitInfo(RaycastHit hitInfo, List<Vector3> validPoints)
        {
            Vector3 start = hitInfo.point + ( hitInfo.normal * characterController.radius * characterRadiusOffset ) + hitInfo.normal;
            Vector3 end = start + ( hitInfo.normal * characterController.height );
                       
            AimRaycastInfo info = new AimRaycastInfo();
            info.hitPoint = hitInfo.point;
            info.normal = hitInfo.normal;
            info.validPoints = validPoints;
            //check if the character can fit in this place
            info.isValid = GetSlopeAngle(info.normal) < slopeLimit && !Physics.CheckCapsule( start, end, characterController.radius * characterRadiusOffset , validLayerMask.value , QueryTriggerInteraction.Ignore );
                       
            return info;
        }
    }
}