using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VRSDK.Locomotion
{
   
    /// Basic class for creating the points of a teleporting line
    /// this is a scritableobject so you can create diferent presets
    public abstract class VR_TeleportAimHandler : ScriptableObject
    {       
        [SerializeField] protected float range = 15.0f;        

        public abstract List<Vector3> GetAllPoints(Ray aimRay);
    }     

}

