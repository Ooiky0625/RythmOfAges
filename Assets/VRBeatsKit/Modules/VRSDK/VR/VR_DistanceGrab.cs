using System;
using UnityEngine;

namespace VRSDK
{
    public class VR_DistanceGrab : MonoBehaviour
    {
        [SerializeField] private Transform pointerTransform = null;
        [SerializeField] private float grabDistance = 5.0f;
        [SerializeField] private float grabRadius = 0.2f;
        [SerializeField] private bool checkForObstruction = false;
        [SerializeField] private bool guideLineAlwaysVisible = false;
        [SerializeField] private bool canTriggerLineRender = false;
        [SerializeField] private VR_InputButton lineTriggerInput = VR_InputButton.Trigger;        
        [SerializeField] private LineRenderer lineRender = null;
        [SerializeField] private LayerMask layerMask = new LayerMask();

        private VR_Controller controller = null;
        private VR_Grabbable activeGrabbable = null;
        private Collider thisCollider = null;      
        public Ray GrabbableRay { get { return new Ray( pointerTransform.position , pointerTransform.forward ); } }
        

        private void Awake()
        {
            controller = GetComponent<VR_Controller>();
            thisCollider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (controller.CurrentGrab != null)
            {
                if (activeGrabbable != null)
                {
                    activeGrabbable.RemoveActiveDistanceGrabController( controller );
                    SetActiveGrabbable(null);
                }                    

                return;
            }


            //get the closer intersection grabbable
            VR_Grabbable closerGrabbable = GrabbableRaycast();
                       

            if (closerGrabbable == null)
            {
                if (activeGrabbable != null)
                {
                    activeGrabbable.RemoveActiveDistanceGrabController(controller);                    
                }

                SetActiveGrabbable( null );

                return;
            }
            if (closerGrabbable != null && activeGrabbable == null)
            {
                SetActiveGrabbable(closerGrabbable);
                activeGrabbable.AddActiveDistanceGrabController(controller);
            }
            else if (closerGrabbable != null && activeGrabbable != null && activeGrabbable != closerGrabbable)
            {
                activeGrabbable.RemoveActiveDistanceGrabController( controller );
                activeGrabbable = closerGrabbable;
                activeGrabbable.AddActiveDistanceGrabController( controller );                
                controller.SetActiveDistanceGrabbable(activeGrabbable);
            }
            else if (closerGrabbable == null && activeGrabbable != null)
            {
                activeGrabbable.RemoveActiveDistanceGrabController(controller);
                SetActiveGrabbable( null );
            }

            

        }

        private VR_Grabbable GrabbableRaycast()
        {

            RaycastHit[] hitArray = Physics.SphereCastAll( GrabbableRay, grabRadius, grabDistance, layerMask, QueryTriggerInteraction.Ignore);
            float minDistance = Mathf.Infinity;
            VR_Grabbable closerGrabbable = null;
            RaycastHit closerHitInfo = new RaycastHit();

            for (int n = 0; n < hitArray.Length; n++)
            {
                float d = hitArray[n].distance;
                VR_Grabbable grabbable = VR_Manager.instance.GetGrabbableFromCollider( hitArray[n].collider );


                if (d < minDistance && IsValidGrabbableRaycast( hitArray[n] ))
                {
                    minDistance = d;
                    closerGrabbable = grabbable;
                    closerHitInfo = hitArray[n];
                }


            }

            if ( ShouldCheckForObstruction(closerGrabbable) )
            {
                Collider col = GetFirstColliderIntersection( pointerTransform.position, ( closerHitInfo.point - pointerTransform.position ).normalized );
                if (col != closerHitInfo.collider)
                {
                    return null;
                }

            }


            return closerGrabbable;
        }

        private bool ShouldCheckForObstruction(VR_Grabbable closerGrabbable)
        {
            return closerGrabbable != null && checkForObstruction;
        }

        private void LateUpdate()
        {
            if ( !HasSomethingToGrab() )
            {
                if ( ShouldRenderLineForward() )
                {
                    RenderLineForward();
                }
                else
                {
                    ClearLineRender();
                }

                return;
            }

            RenderLineToActiveGrabbable();
           
        }

        private bool HasSomethingToGrab()
        {
            return activeGrabbable != null;
        }

        private bool ShouldRenderLineForward()
        {
            return controller.CurrentGrab == null && ( guideLineAlwaysVisible || ( canTriggerLineRender && controller.Input.GetButton( lineTriggerInput ) ) );
        }

        private void RenderLineForward()
        {
            lineRender.useWorldSpace = false;
            lineRender.positionCount = 2;
            lineRender.SetPosition( 0, Vector3.zero );
            lineRender.SetPosition( 1, Vector3.forward * ( grabDistance / transform.localScale.z ) );
        }

        private void OnDisable()
        {
            if (lineRender != null)
            {
                lineRender.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (lineRender != null)
            {
                lineRender.enabled = true;
            }
        }

        private void RenderLineToActiveGrabbable()
        {
            Transform lineEnd = GetLineEndTransform();

            if (lineEnd == null)
                return;

            lineRender.useWorldSpace = true;
            lineRender.positionCount = 2;
            lineRender.SetPosition( 0, pointerTransform.position );
            lineRender.SetPosition( 1, lineEnd.position );
        }

        private Transform GetLineEndTransform()
        {
            Transform lineEnd = activeGrabbable.GetHandInteractionSettings(controller).highlightPoint;

            if (lineEnd == null)
                lineEnd = activeGrabbable.GetHandInteractionSettings(controller).interactPoint;


            return lineEnd;
        }

        private void ClearLineRender()
        {
            lineRender.positionCount = 0;
        }
       

        private Collider GetFirstColliderIntersection(Vector3 origin , Vector3 dir)
        {
            RaycastHit hitInfo;

            if (Physics.Raycast( origin, dir, out hitInfo, grabDistance , layerMask , QueryTriggerInteraction.Ignore))
                return hitInfo.collider;

            return null;
        }
        

        private bool IsValidGrabbableRaycast(RaycastHit hitInfo)
        {
            VR_Grabbable grabbable = VR_Manager.instance.GetGrabbableFromCollider(hitInfo.collider);
            return hitInfo.collider != thisCollider && grabbable != null && grabbable.enabled && grabbable.UseDistanceGrab && grabbable.CurrentGrabState == GrabState.UnGrab;
        }

        private void SetActiveGrabbable(VR_Grabbable grabbable)
        {
            activeGrabbable = grabbable;
            controller.SetActiveDistanceGrabbable( grabbable );
        }

        
    }
}

