using UnityEngine;
using System.Collections.Generic;

namespace VRSDK
{
    public class HandPhysics
    {
        private HistoryBuffer historyBuffer = null;        
        private const float throwSmoothVelocity = 3.0f;
        private const float velocityModifier = 1.9f;
        private const float angularVeloctyModifier = 1.8f;
        private const int defaultSampleSize = 5;
        private const int handDirectionSampleSize = 5;
        private VR_CharacterController characterController = null;     

        private Transform trackingSpace = null; 


        public Vector3 Velocity
        {
            get
            {
                List<Vector3> velocityHistory = historyBuffer.VelocityHistory.Sample( defaultSampleSize );

                Vector3 throwDirection = CalculateThrowDirection();
                
                //calculate the EMA (Exponential Moving Average), just a way to predict the desire throw velocity base on previus velocities
                //you can read more here https://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average
                float velocityMagnitudeEMA = CalculateMagnitudeEMA( velocityHistory, throwSmoothVelocity );

               
                return throwDirection * velocityMagnitudeEMA * velocityModifier;
            }
        }

        public Vector3 AngularVelocity
        {
            get
            {
                List<Vector3> angularVelocityHistory = historyBuffer.AngularVelocityHistory.Sample( defaultSampleSize );

                //calculate the EMA (Exponential Moving Average), just a way to predict the desire throw velocity base on previus velocities
                //you can read more here https://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average
                return CalculateEMA( angularVelocityHistory, throwSmoothVelocity ) * angularVeloctyModifier;
            }
        }


        public HandPhysics(HistoryBuffer buffer)
        {
            historyBuffer = buffer;
            characterController = MonoBehaviour.FindObjectOfType<VR_CharacterController>();
        
            trackingSpace = VR_Manager.instance.Player.TrackingSpace;
          
        }

        public void ApplyThrowVelocity(VR_Grabbable grabbable)
        {
          
            Rigidbody rb = grabbable.RB;                  
            
            //apply the hand velocity and angular velocity
            ProcessThrowVelocity(rb);
            ProcessThrowAngularVelocity(rb);
        }

        private void ProcessThrowVelocity(Rigidbody rb)
        {            
            //add the throw velocity
            rb.AddForce( Velocity, ForceMode.VelocityChange );

            if (characterController != null)
            {
                //add the character controller velocity, maybe this dont made to much sense at first but helps a lot when you throw a object while moving
                rb.AddForce( characterController.Velocity, ForceMode.VelocityChange );
            }
           
        }

        private void ProcessThrowAngularVelocity(Rigidbody rb)
        {
            rb.AddTorque( AngularVelocity , ForceMode.VelocityChange );
        }


        private Vector3 CalculateThrowDirection()
        {
            List<Vector3> localPositionHistory = historyBuffer.LocalPositionHistory.Sample( handDirectionSampleSize );

            Vector3 throwDirection = (  localPositionHistory[localPositionHistory.Count - 1] - localPositionHistory[0] ).normalized;

            if (trackingSpace != null)
            {
                return trackingSpace.TransformDirection( throwDirection );
            }

            return throwDirection;
        }

#region MATH
        //you can read more here https://en.wikipedia.org/wiki/Moving_average
        private float CalculateMagnitudeSMA(List<Vector3> buffer)
        {
            Vector3 sum = Vector3.zero;

            for (int index = 0; index < buffer.Count - 1; index++)
            {
                sum += buffer[index];
            }

            return ( sum / buffer.Count ).magnitude;
        }

        private Vector3 CalculateSMA(List<Vector3> buffer)
        {
            Vector3 sum = Vector3.zero;

            for (int index = 0; index < buffer.Count - 1; index++)
            {
                sum += buffer[index];
            }

            return ( sum / buffer.Count );
        }

        private Vector3 CalculateEMA(List<Vector3> buffer , float modifier)
        {
            Vector3 SMA = CalculateSMA(buffer);
            float smoothingConst = 2 / ( defaultSampleSize + 1 );
            Vector3 EMA = ( buffer[buffer.Count - 1] - SMA ) * ( smoothingConst * modifier );
            EMA += SMA;

            return EMA;
        }

        private float CalculateMagnitudeEMA(List<Vector3> buffer, float modifier)
        {
            float SMA = CalculateMagnitudeSMA( buffer );
            float smoothingConst = 2 / ( defaultSampleSize + 1 );
            float EMA = ( buffer[buffer.Count - 1].magnitude - SMA ) * ( smoothingConst * modifier );
            EMA += SMA;

            return EMA;
        }

#endregion

    }
}

