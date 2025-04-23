using UnityEngine;
using VRSDK.Collections;

namespace VRSDK
{
    public class HistoryBuffer : MonoBehaviour
    {
        [SerializeField] private int bufferSize = 10;

        private Buffer<Vector3> velocityHistory;
        private Buffer<Vector3> angularVelocityHistory;
        private Buffer<Vector3> localPositionHistory;
        private Buffer<Vector3> positionHistory;
        private Buffer<Quaternion> rotationHistory;
        private Vector3 lastLocalPosition = Vector3.zero;
       

        public Buffer<Vector3> PositionHistory { get { return positionHistory; } }
        public Buffer<Vector3> LocalPositionHistory { get { return localPositionHistory; } }
        public Buffer<Vector3> AngularVelocityHistory { get { return angularVelocityHistory; } }
        public Buffer<Quaternion> RotationHistory { get { return rotationHistory; } }
        public Buffer<Vector3> VelocityHistory { get { return velocityHistory; } }

        
        private void Awake()
        {
            velocityHistory = new Buffer<Vector3>( bufferSize );
            angularVelocityHistory = new Buffer<Vector3>( bufferSize );
            localPositionHistory = new Buffer<Vector3>( bufferSize );
            positionHistory = new Buffer<Vector3>( bufferSize );
            rotationHistory = new Buffer<Quaternion>(bufferSize);
           
            lastLocalPosition = transform.localPosition;           
        }

        private void Update()
        {
            UpdateVelocityHistory();
            UpdateAngularVelocityHistory();
            UpdateLocalPositionHistory();
            UpdatePositionHistory();
            UpdateRotationHistory();
        }

        private void UpdateVelocityHistory()
        {
            velocityHistory.Add( (lastLocalPosition - transform.localPosition) / Time.deltaTime );
            lastLocalPosition = transform.localPosition;
        }

        private void UpdateRotationHistory()
        {
            rotationHistory.Add( transform.rotation );
        }

        private void UpdateAngularVelocityHistory()
        {
            if (rotationHistory.Count < 2)
                return;

            float delta = Time.deltaTime;
            float angleDegrees = 0.0f;
            Vector3 unitAxis = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            rotation = ( rotationHistory[rotationHistory.Count - 1] ) * Quaternion.Inverse( rotationHistory[rotationHistory.Count - 2] );

            rotation.ToAngleAxis( out angleDegrees, out unitAxis );
            Vector3 angular = unitAxis * ( ( angleDegrees * Mathf.Deg2Rad ) / delta );

            angularVelocityHistory.Add( angular);

        }
        private void UpdateLocalPositionHistory()
        {
            localPositionHistory.Add(transform.localPosition);
        }

        private void UpdatePositionHistory()
        {
            positionHistory.Add( transform.position );
        }
       
    }

    
}

