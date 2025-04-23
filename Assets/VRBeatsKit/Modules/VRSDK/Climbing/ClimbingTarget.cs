using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VRSDK.Climbing
{
    /// <summary>
    /// Use this script in the GameObject being affected by climbing in our case the Player gameObject
    /// </summary>
    public class ClimbingTarget : MonoBehaviour
    {
        [SerializeField] private UnityEvent onClimbStart = null;
        [SerializeField] private UnityEvent onClimbEnd = null;

        private List<ClimbPoint> climbPointList = new List<ClimbPoint>();
        private ClimbPoint activeClimbPoint = null;
        
        public UnityEvent OnClimbStart { get { return onClimbStart; } }
        public UnityEvent OnClimbEnd { get { return onClimbEnd; } }

        private void Update()
        {
            if (activeClimbPoint != null)
            {
                activeClimbPoint.SetClimbingPosition();
            }
        }

        private void OnClimbPointReleased(ClimbPoint climbPoint)
        {
            climbPointList.Remove(climbPoint);

            if (activeClimbPoint == climbPoint)
            {
                activeClimbPoint = GetActiveClimbPoint();

                if (activeClimbPoint != null)
                {
                    activeClimbPoint.OnClimbPointActive();
                }
                else
                {
                    onClimbEnd.Invoke();
                }
            }

                                   
        }

        public void AddActiveClimbPoint(ClimbPoint climbPoint)
        {
            climbPointList.Add(climbPoint);

            climbPoint.OnGrabStateChange.AddListener( delegate(GrabState state) 
            {
                if (state == GrabState.Drop)
                {
                    OnClimbPointReleased(climbPoint);
                }
            } );

            if (activeClimbPoint == null)
            {               
                activeClimbPoint = GetActiveClimbPoint();

                if (activeClimbPoint != null)
                {
                    activeClimbPoint.OnClimbPointActive();
                    onClimbStart.Invoke();
                }
                    
            }               

        }

        private ClimbPoint GetActiveClimbPoint()
        {
            if (climbPointList.Count == 0)
                return null;

            return climbPointList[0];
        }

    }
}
