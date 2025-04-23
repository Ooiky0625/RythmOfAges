using UnityEngine;
using System.Collections.Generic;

namespace VRSDK.Locomotion
{
    //this is a preset for a parabolic teleport
    [CreateAssetMenu(fileName = "ParabolicTeleporPreset", menuName = "VRShooterKit/Create Parabolic Teleport Preset")]
    public class VR_ParabolicAimHandler : VR_TeleportAimHandler
    {
        [SerializeField] protected float projectileSpeed = 15.0f;
        [SerializeField] protected float gravity = 9.8f;
        [Tooltip("The fps for the fake projectile use for generate a parabolic line, more fps will produce more points so be careful")]
        [Range(10 , 75)]
        [SerializeField] protected int simulatedFPS = 40; 

       
        public override List<Vector3> GetAllPoints(Ray aimRay)
        {
            //calculate the fake delta time
            float deltaTime = 1.0f / (float) simulatedFPS;

            //calculate initial speed
            Vector3 m = aimRay.direction * projectileSpeed * deltaTime;
            Vector3 projectilePos = aimRay.origin;

            List<Vector3> points = new List<Vector3>();


            //we calculate the distance of the fake projectil using just x,z so it looks better
            float squareRange = range * range;
            Vector2 origin = new Vector2(aimRay.origin.x , aimRay.origin.z);

            while ( (origin - new Vector2( projectilePos.x , projectilePos.z )).sqrMagnitude < squareRange)
            {               
                points.Add( projectilePos );
                //add gravity
                m -= Vector3.up * gravity * deltaTime;
                //move our fake projectil
                projectilePos += m;
            }

            return points;
        }
    }

}

