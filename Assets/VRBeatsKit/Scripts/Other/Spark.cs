using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBeats
{
    public class Spark : MonoBehaviour
    {
        [SerializeField] MaterialBindings materialBindings = null;
        [SerializeField] private float glowEffect = 0.5f;
        [SerializeField] private float scaleTime = 1.5f;
        [SerializeField] private float moveTime = 1.5f;
        [SerializeField] private float targetScale = 500.0f;

        public void Construct(Color c)
        {
            materialBindings.SetUseEmmisiveIntensity(false);
            materialBindings.SetEmmisiveColor(c * glowEffect);
            PlayAnimation();
        }

        private void OnDestroy()
        {
            gameObject.CancelAllTweens();
        }

        private void PlayAnimation()
        {
            transform.ScaleY(targetScale, scaleTime).SetEase(Ease.EaseOutExpo).SetOnComplete(delegate
          {
              transform.Move(transform.position + Vector3.up * targetScale, moveTime).SetOnComplete(delegate
            {
                  Destroy(gameObject);
              }).SetOwner(gameObject);
          }).SetOwner(gameObject); ;
        }

    }

}
