using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platinio.TweenEngine;

namespace VRBeats
{
    public class RotationLoop : MonoBehaviour
    {
        [SerializeField] private float time = 0.0f;
        [SerializeField] private float delay = 0.1f;
        [SerializeField] private Ease ease = Ease.Linear;
        [SerializeField] private Vector3 axis = Vector3.up;
        

        private const float speed = 360.0f;

        private void Awake()
        {
            Rotate();
        }

        private void Rotate()
        {
            transform.RotateTween( Vector3.up , 360.0f , time).SetOnComplete(Rotate).SetDelay(delay).SetEase(ease);
            /*
            PlatinioTween.instance.ValueTween(0.0f , 1.0f , time).SetOnUpdateFloat( delegate (float v)
            {
                //transform.rotation = Quaternion.Lerp( transform.rotation , Quaternion.Euler( axis * 360.0f ) , v );
                transform.Rotate(axis , speed * v * Time.deltaTime);
            } ).SetOnComplete( Rotate ).SetDelay(delay).SetEase(ease);*/
        }

    }
}

