using UnityEngine;
using System.Collections;

namespace VRSDK
{
    //wall cube demo code
    public class WallCubePart : MonoBehaviour
    {
        private Vector3 startPosition = Vector3.zero;
        private Quaternion startRotation = Quaternion.identity;
        private Rigidbody rb = null;

        private void Awake()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
            rb = GetComponent<Rigidbody>();
        }

        public void Reset(float t)
        {
            rb.isKinematic = true;
           
            StartCoroutine(MoveRoutine(t));
            StartCoroutine( RotateRoutine (t));
            
        }

        private IEnumerator MoveRoutine(float t)
        {
            float currentTime = 0.0f;

            Vector3 currentPosition = transform.position;

            while (currentTime < t)
            {
                currentTime += Time.deltaTime;
                transform.position = Vector3.Lerp( currentPosition, startPosition, currentTime / t );
                yield return new WaitForEndOfFrame();
            }

            transform.position = startPosition;

            yield return new WaitForSeconds( 1.0f );
            rb.isKinematic = false;
        }
       
        private IEnumerator RotateRoutine(float t)
        {
            float currentTime = 0.0f;

            Quaternion currentRotation = transform.rotation;

            while (currentTime < t)
            {
                currentTime += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(currentRotation , startRotation , currentTime / t);
                yield return new WaitForEndOfFrame();
            }

            transform.rotation = startRotation;
        }
       

    }
}

