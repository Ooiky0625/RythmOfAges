using UnityEngine;

namespace VRBeats
{
    public class RandomizeRotation : MonoBehaviour
    {
        [SerializeField] private float minRotation = 0.0f;
        [SerializeField] private float maxRotation = 0.0f;
        [SerializeField] private Ease ease = Ease.EaseOutExpo;
        [SerializeField] private float animTime = 2.0f;

        private void Awake()
        {
            animTime = Random.Range(animTime / 2.0f , animTime);

            Rotate();
        }

        private void Rotate()
        {
            float rotation = Random.Range(-maxRotation , maxRotation);
            transform.RotateTween( Vector3.forward , rotation , animTime).SetEase(ease).SetOnComplete( Rotate );
        }

    } 

}
