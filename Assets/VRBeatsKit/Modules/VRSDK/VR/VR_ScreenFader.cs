using System.Collections;
using UnityEngine;
using System;

namespace VRSDK
{
    public class VR_ScreenFader : MonoBehaviour
    {
      
        [Tooltip( "Screen color at maximum fade" )]
        public Color fadeColor = new Color( 0.01f, 0.01f, 0.01f, 1.0f );

        [SerializeField] private float startAlpha = 0.0f;

       
        /// <summary>
        /// The render queue used by the fade mesh. Reduce this if you need to render on top of it.
        /// </summary>
        public int renderQueue = 5000;

        private float uiFadeAlpha = 0;

        private MeshRenderer fadeRenderer;
        private MeshFilter fadeMesh;
        private Material fadeMaterial = null;
        private bool isFading = false;


        protected virtual void Start()
        {
            // create the fade material
            fadeMaterial = new Material( Shader.Find( "VR/Unlit Transparent Color" ) );
            fadeMesh = gameObject.GetComponent<MeshFilter>();

            if (fadeMesh == null)
                fadeMesh = gameObject.AddComponent<MeshFilter>();

            fadeRenderer = gameObject.GetComponent<MeshRenderer>();

            if (fadeRenderer == null)
                fadeRenderer = gameObject.AddComponent<MeshRenderer>();

            

            var mesh = new Mesh();
            fadeMesh.mesh = mesh;

            Vector3[] vertices = new Vector3[4];

            float width = 2f;
            float height = 2f;
            float depth = 1f;

            vertices[0] = new Vector3( -width, -height, depth );
            vertices[1] = new Vector3( width, -height, depth );
            vertices[2] = new Vector3( -width, height, depth );
            vertices[3] = new Vector3( width, height, depth );

            mesh.vertices = vertices;

            int[] tri = new int[6];

            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            mesh.triangles = tri;

            Vector3[] normals = new Vector3[4];

            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;

            mesh.normals = normals;

            Vector2[] uv = new Vector2[4];

            uv[0] = new Vector2( 0, 0 );
            uv[1] = new Vector2( 1, 0 );
            uv[2] = new Vector2( 0, 1 );
            uv[3] = new Vector2( 1, 1 );

            mesh.uv = uv;

            SetMaterialAlpha(startAlpha);
        }                     
       

        /// <summary>
        /// Cleans up the fade material
        /// </summary>
        void OnDestroy()
        {
            if (fadeRenderer != null)
                Destroy( fadeRenderer );

            if (fadeMaterial != null)
                Destroy( fadeMaterial );

            if (fadeMesh != null)
                Destroy( fadeMesh );
        }

        public void FadeOut(float time , Action onComplete = null)
        {
            StartCoroutine( Fade( 1.0f , 0.0f , time , onComplete ) );
        }

        public void FadeIn(float time, Action onComplete = null)
        {
            StartCoroutine( Fade( 0.0f, 1.0f, time, onComplete ) );
        }
       
        /// <summary>
        /// Fades alpha from 1.0 to 0.0
        /// </summary>
        public IEnumerator Fade(float startAlpha, float endAlpha , float time , Action onComplete = null)
        {
            float elapsedTime = 0.0f;
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                float currentAlpha = Mathf.Lerp( startAlpha, endAlpha, Mathf.Clamp01( elapsedTime / time ) );
                SetMaterialAlpha(currentAlpha);
                yield return new WaitForEndOfFrame();
            }

            if (onComplete != null)
                onComplete();
        }

        /// <summary>
        /// Update material alpha. UI fade and the current fade due to fade in/out animations (or explicit control)
        /// both affect the fade. (The max is taken) 
        /// </summary>
        public void SetMaterialAlpha(float alpha)
        {
            Color color = fadeColor;
            color.a = Mathf.Max( alpha, uiFadeAlpha );
            isFading = color.a > 0;
            if (fadeMaterial != null)
            {
                fadeMaterial.color = color;
                fadeMaterial.renderQueue = renderQueue;
                fadeRenderer.material = fadeMaterial;
                fadeRenderer.enabled = isFading;
            }
        }
    }

}

