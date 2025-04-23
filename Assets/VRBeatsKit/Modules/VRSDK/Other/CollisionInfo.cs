using UnityEngine;
using System.Collections.Generic;

namespace VRSDK
{
    public class CollisionInfo : MonoBehaviour
    {
        [SerializeField] private List<Collider> contactColliderList = new List<Collider>();

        public List<Collider> GetCurrentContactColliders()
        {
            return contactColliderList;
        }

        public bool IsInCollision()
        {
            return contactColliderList.Count > 0;
        }
        private void OnCollisionEnter(Collision other)
        {           
            contactColliderList.Add(other.collider);
        }

        private void OnCollisionExit(Collision other)
        {
            contactColliderList.Remove( other.collider );
        }

        
    }
}

