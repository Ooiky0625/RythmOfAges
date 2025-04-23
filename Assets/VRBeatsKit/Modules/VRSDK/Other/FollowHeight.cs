using UnityEngine;

namespace VRSDK
{
    public class FollowHeight : MonoBehaviour
    {
        [SerializeField] private Transform objToFollow = null;

        private void Update()
        {
            transform.position = objToFollow.position;
        }

        private void LateUpdate()
        {
            /*
            Vector3 currentPos = transform.position;
            currentPos.y = objToFollow.position.y;
            transform.position = currentPos;*/
            transform.position = objToFollow.position;
        }

        private void FixedUpdate()
        {
            transform.position = objToFollow.position;
        }
    }
}

