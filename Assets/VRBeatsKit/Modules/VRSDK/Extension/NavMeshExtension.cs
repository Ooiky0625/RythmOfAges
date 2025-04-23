using UnityEngine.AI;
using UnityEngine;

namespace VRSDK.AI
{
    public static class NavMeshExtension 
    {
        public static Vector3 CalculateRandomPointInsideNavMesh(Vector3 center , float radius , int areaMask)
        {
            Vector3 point = center + Random.insideUnitSphere * radius;            
            NavMeshHit hit;

            if (NavMesh.SamplePosition( point, out hit, radius, areaMask ))
            {
                return hit.position;
            }

            return center;
        }
    }

}

