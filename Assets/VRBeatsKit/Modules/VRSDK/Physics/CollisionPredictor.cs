using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VRSDK
{
    /// <summary>
    /// Collision predictor help us to cast complex collider shapes and check for posibles collisions
    /// </summary>
    public class CollisionPredictor : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask = new LayerMask();

        private List<Collider> colliderList = null;
        private Transform collisionPredictorParent = null;
        private bool collisionCopyCreate = false;
               

        private void LateUpdate()
        {
            if (!collisionCopyCreate)
            {
                CreateCollisionCopy();
                SetLayerMask();
                RemoveInvalidColliders();
                collisionCopyCreate = true;
            }
        }

        private void CreateCollisionCopy()
        {
            GameObject clone = Instantiate( gameObject );
            RemoveComponents( clone );

            if (clone.GetComponent<VR_OutlineHighlight>() != null)
            {
                Destroy( clone.GetComponent<VR_OutlineHighlight>() );
            }

            if (clone.GetComponent<VR_Outline>() != null)
            {
                Destroy( clone.GetComponent<VR_Outline>() );
            }            

            clone.name = gameObject.name + "_CollisionPredictorCopy";
            clone.transform.localScale = clone.transform.localScale;
            collisionPredictorParent = clone.transform;
            colliderList = clone.GetComponentsInChildren<Collider>().ToList();            
        }

        private void SetLayerMask()
        {
            for (int n = 0; n < colliderList.Count; n++)
            {
                colliderList[n].gameObject.layer = LayerMask.NameToLayer( "IgnoreCollision" );
            }
        }

        private void RemoveInvalidColliders()
        {
            for (int n = 0; n < colliderList.Count; n++)
            {
                if (colliderList[n] is MeshCollider)
                {
                    Debug.LogWarning("Collider " + colliderList[n].name + " is a MeshCollider, CollisionPredictor does no support MeshColliders");
                    Destroy(colliderList[n]);
                    colliderList.RemoveAt( n );
                    n--;
                }

                else if (colliderList[n].isTrigger)
                {
                    Debug.LogWarning("Collider " + colliderList[n].name + " is a trigger collider, removing it from CollisionPredictor");
                    Destroy( colliderList[n] );
                    colliderList.RemoveAt( n );
                    n--;
                }
            }
        }

        public bool WillCollisionAtPositionAndRotation(Vector3 position , Quaternion rotation )
        {
            //we need to create first our collision copy
            if (!collisionCopyCreate)
                return false;

            //trasnlate the object to the desire postion and rotation
            collisionPredictorParent.position = position;
            collisionPredictorParent.rotation = rotation;

            for (int n = 0; n < colliderList.Count; n++)
            {
                if (CheckCollisionAtPosition( colliderList[n] ))
                    return true;
            }

            return false;
        }
        
        
        private bool CheckCollisionAtPosition(Collider collider)
        {
            if (collider is SphereCollider)
            {
                return PhysicsExtensions.CheckSphere(collider as SphereCollider , layerMask , QueryTriggerInteraction.Ignore);
            }
            if (collider is BoxCollider)
            {
                return PhysicsExtensions.CheckBox( collider as BoxCollider, layerMask, QueryTriggerInteraction.Ignore );
            }
            if (collider is CapsuleCollider)
            {
                return PhysicsExtensions.CheckCapsule(collider as CapsuleCollider , layerMask , QueryTriggerInteraction.Ignore);
            }

            return false;
        }


        private void RemoveComponents(GameObject go)
        {
            Component[] componentArray = go.GetComponentsInChildren<Component>();

            for (int n = 0; n < componentArray.Length; n++)
            {
                if (componentArray[n] != null)
                {
                    if (componentArray[n] is Canvas)
                        Destroy( componentArray[n].gameObject );

                    else if (CanDestroyComponent( componentArray[n] ))
                        Destroy( componentArray[n] );
                }


            }
        }

        private bool CanDestroyComponent(Component c)
        {
            return !( c is Transform ) && !(c is Collider) && !( c is VR_OutlineHighlight) && !( c is VR_Outline );
        }

    }
}

