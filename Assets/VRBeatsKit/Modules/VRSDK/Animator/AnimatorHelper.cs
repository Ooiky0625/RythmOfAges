using UnityEngine;
using System.Collections.Generic;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace VRSDK
{
    /// <summary>
    /// This scripts helps handling the Animator
    /// </summary>
    public class AnimatorHelper : MonoBehaviour
    {
        [SerializeField] private List<LayerInfo> animatorControllerInfo = null;
        [SerializeField] private float layerTransitionTime = 0.15f;

        private Animator animator = null;        
        private Coroutine changeLayerValueCoroutine = null;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Do smooth layer transition
        /// </summary>       
        public void EnableLayer(int layer)
        {
            if (changeLayerValueCoroutine != null)
                StopCoroutine( changeLayerValueCoroutine );

            changeLayerValueCoroutine = StartCoroutine( ChangeLayerValueRoutine( layer, 1.0f, layerTransitionTime ) );
        }

        /// <summary>
        /// Do smooth layer transition
        /// </summary>    
        public void DisableLayer(int layer)
        {
            if (changeLayerValueCoroutine != null)
                StopCoroutine( changeLayerValueCoroutine );

            changeLayerValueCoroutine = StartCoroutine( ChangeLayerValueRoutine( layer, 0.0f, layerTransitionTime ) );
        }

        private IEnumerator ChangeLayerValueRoutine(int layer, float v, float t)
        {
            float timer = 0.0f;

            while (timer < t)
            {
                float layerWeight = animator.GetLayerWeight( layer );
                animator.SetLayerWeight( layer, Mathf.Lerp( layerWeight, v, timer / t ) );

                timer += Time.deltaTime;

                yield return new WaitForSeconds( Time.deltaTime );
            }

            animator.SetLayerWeight( layer, v );
        }

        /// <summary>
        /// This method is really helpfull, you can know whethever the animator is playing certain state or trasition to it
        /// </summary>        
        public bool IsPlayingOrTransitionToState(int layer , string stateName)
        {
            return IsPlayingState( layer, stateName ) || IsTransitionToState(layer , stateName);
        }

        /// <summary>
        /// is the state name in layer being played?
        /// </summary>       
        public bool IsPlayingState(int layer , string stateName)
        {            
            return animator.GetCurrentAnimatorStateInfo( layer ).IsName( stateName );
        }

        /// <summary>
        /// Is animator trasition to current state in layer
        /// </summary>       
        public bool IsTransitionToState(int layer , string stateName)
        {
            List<string> states = animatorControllerInfo[layer].animatorStates;

            for (int n = 0; n < states.Count; n++)
            {
                if (animator.GetAnimatorTransitionInfo( layer ).IsName( states[n] + " -> " + stateName ))
                    return true;
            }

            return false;
        }

#if UNITY_EDITOR
        /// in order to know if the animator is trasition to another state we need to know all state names before hand
        /// but we can only know them using using UnityEditor.Animations namespace, so we ned to build this information in Editor
        /// and serialized it so we can access it later in running time, this method is called in I_AnimatorHelperInspector.cs in the Awake
        public void ConstructAnimatorControllerInfo()
        {
            animatorControllerInfo = new List<LayerInfo>();

            AnimatorController ac = GetComponent<Animator>().runtimeAnimatorController as AnimatorController;

            AnimatorControllerLayer[] layerArray = ac.layers;

            for (int n = 0; n < layerArray.Length; n++)
            {
                LayerInfo info = new LayerInfo();
                info.animatorStates = GetStatesFromLayer( layerArray[n] ); ;

                animatorControllerInfo.Add(info);
            }
        }

        private List<string> GetStatesFromLayer(AnimatorControllerLayer layer)
        {
            List<string> states = new List<string>();

            for (int n = 0; n < layer.stateMachine.states.Length; n++)
            {
                states.Add( layer.stateMachine.states[n].state.name );
            }

            return states;
        }
#endif

    }
    /// <summary>
    /// serialize class for storing animator state names
    /// </summary>
    [System.Serializable]
    public class LayerInfo
    {
        public List<string> animatorStates = new List<string>();
    }
}

