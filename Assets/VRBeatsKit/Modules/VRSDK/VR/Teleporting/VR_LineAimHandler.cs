using System.Collections.Generic;
using UnityEngine;

namespace VRSDK.Locomotion
{
    //this is a preset for line teleport
    [CreateAssetMenu( fileName = "LineTeleporPreset", menuName = "VRShooterKit/Create Line Teleport Preset" )]
    public class VR_LineAimHandler : VR_TeleportAimHandler
    {
        public override List<Vector3> GetAllPoints(Ray aimRay)
        {
            Vector3 start = aimRay.origin;
            Vector3 end = start + ( aimRay.direction * range );

            return new List<Vector3>() {start , end };
        }
    }
}

