using UnityEngine;
using System.Collections;

namespace VRBeats 
{
    public class RotPartController : MonoBehaviour
    {
        [SerializeField] private Ease ease = Ease.Linear;
        [SerializeField] private float animTime = 2.0f;

        private Transform[] childArray = null;

        private bool open = false;

        private void Awake()
        {
            childArray = new Transform[ transform.childCount ];

            for (int n = 0; n < transform.childCount; n++)
            {
                childArray[n] = transform.GetChild(n);
            }

            //PlayRotAnimation(10.0f);
            //yield return new WaitForSeconds(10.0f);
             //Reverse(10.0f);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (open)
                {
                    Reverse(0.0f);
                }
                else
                {
                    PlayRotAnimation(10.0f);
                }

                open = !open;
            }
                
        }

        public void PlayRotAnimation(float rotAmount)
        {
            float step = rotAmount;

            for (int n = 0; n < childArray.Length; n++)
            {
                childArray[n].RotateTween(Vector3.forward , step * ( childArray.Length - n ) , animTime ).SetEase(ease);
                childArray[n].ScaleTween( new Vector3(1.0f , 1.0f , 0.0f) , animTime ).SetEase(ease);
            }

        }

        public void Reverse(float rotAmount)
        {
            float step = rotAmount;

            for (int n = 0; n < childArray.Length; n++)
            {
                childArray[n].RotateTween(Vector3.forward, step * (childArray.Length - n), animTime).SetEase(ease);
                childArray[n].ScaleTween(new Vector3(1.0f, 1.0f, 4.0f), animTime).SetEase(ease);
            }
        }

    }

}

