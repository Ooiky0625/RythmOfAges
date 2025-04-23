using UnityEngine;
using System.Collections;

namespace VRSDK
{
    public class VR_UIHighlight : VR_Highlight
    {
        #region INSPECTOR 
        [SerializeField] private Canvas canvas = null;
        [SerializeField] private Transform grabbableMarker = null;
        [SerializeField] private float scaleTime = 0.2f;
        [SerializeField] private float radius = 0.2f;
        #endregion

        private Coroutine scaleCoroutine = null;

        private void Update()
        {
            LookAtCamera();


            if (CanHighlight())
            {
                if (!canvas.gameObject.activeInHierarchy)
                    canvas.gameObject.SetActive( true );

            }
            else
            {
                if (canvas.gameObject.activeInHierarchy)
                    canvas.gameObject.SetActive( false );

                grabbableMarker.localScale = Vector3.one;
            }

        }

        private void LookAtCamera()
        {
            canvas.transform.position = canvas.transform.parent.position + ( ( Camera.main.transform.position - transform.position ).normalized * radius );
        }

        public override void Highlight(VR_Controller controller)
        {
            ScaleTween( 1.2f );
        }

        public override void UnHighlight(VR_Controller controller)
        {
            ScaleTween( 1.0f );
        }

        private void ScaleTween(float scale)
        {           
            if (scaleCoroutine != null)
                StopCoroutine( scaleCoroutine );

            scaleCoroutine = StartCoroutine( ScaleRoutine( scale ) );

        }

        private IEnumerator ScaleRoutine(float scale)
        {
            float currentTime = 0.0f;

            while (currentTime <= scaleTime)
            {
                currentTime += Time.deltaTime;
                grabbableMarker.transform.localScale = Vector3.Lerp( grabbableMarker.transform.localScale , Vector3.one * scale , currentTime / scaleTime );

                yield return new WaitForEndOfFrame();
            }

            grabbableMarker.transform.localScale = Vector3.one * scale;
        }

    }

}

