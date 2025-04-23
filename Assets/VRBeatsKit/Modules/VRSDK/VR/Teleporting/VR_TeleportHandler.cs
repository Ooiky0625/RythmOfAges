using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace VRSDK.Locomotion
{
    //this script handles the position change part of the teleport system
    public class VR_TeleportHandler : MonoBehaviour
    {
        [SerializeField] private VR_ScreenFader screenFader = null;
        [SerializeField] private bool useBlink = true;
        [SerializeField] private float blinkTime = 0.5f;
        [SerializeField] private UnityEvent onTeleport = null;

        private Vector3 characterPosition = Vector3.zero;
        private Vector3 teleportFoward = Vector3.zero;
        private CharacterController affectedCharacterController = null;
                
        public UnityEvent OnTeleport { get { return onTeleport;  } }
        
        /// Teleport a charactercontroller to a new position and rotation            
        public void DoTeleport(CharacterController characterController , Transform to , Action onTeleportComplete)
        {
            affectedCharacterController = characterController;
            characterPosition = to.position + ( Vector3.up * characterController.height * 0.5f ) + (characterController.center * -1.0f);
            teleportFoward = to.forward;
            teleportFoward.y = 0.0f;
           

            if (useBlink)
            {
                StartCoroutine( TeleportRoutine(onTeleportComplete) );
            }
            else
            {
                SetTeleportPositionAndRotation();                

                if (onTeleportComplete != null)
                    onTeleportComplete();
            }
        }
        
        //we use a routine if we want to use a screen fading effect
        private IEnumerator TeleportRoutine(Action onTeleportComplete)
        {            
            yield return StartCoroutine( screenFader.Fade(0.0f , 1.0f , blinkTime) );
            SetTeleportPositionAndRotation();
            onTeleport.Invoke();
            yield return StartCoroutine( screenFader.Fade( 1.0f, 0.0f, blinkTime ) );

            if (onTeleportComplete != null)
                onTeleportComplete();
        }

        //set final teleport position and rotation
        private void SetTeleportPositionAndRotation()
        {
            affectedCharacterController.transform.position = characterPosition;
            affectedCharacterController.transform.forward = teleportFoward;
        }

       
    }
}

