using UnityEngine;
using System.Collections.Generic;

namespace VRSDK.Locomotion
{
    public enum TeleporState
    {
        WaitingInput,
        PreTeleport,
        PostTeleport
    }

    //this script makes use of all the teleport components and handles the logic for teleporting
    public class VR_TeleporManager : MonoBehaviour
    {
        [SerializeField] private VR_LineRenderer teleportLineRender = null;
        [SerializeField] private VR_TeleportAimHandler aimHandler = null;
        [SerializeField] private VR_AimRaycaster aimRaycaster = null;
        [SerializeField] private VR_AimMarker aimMarker = null;
        [SerializeField] private VR_TeleportHandler teleportHandler = null;
        [SerializeField] private CharacterController characterController = null;       
        

        private VR_Controller leftController = null;
        private VR_Controller rightController = null;
        private VR_Controller activeController = null;
        private VR_Controller lastActiveController = null;
        private AimRaycastInfo lastRaycastInfo = null;
        private TeleporState currentTeleportState = TeleporState.WaitingInput;
        private bool isTeleporting = false;

        private void Start()
        {
            rightController = VR_Manager.instance.Player.RightController;
            leftController = VR_Manager.instance.Player.LeftController;
        }       

        private void LateUpdate()
        {           

            switch (currentTeleportState)
            {
                case TeleporState.WaitingInput:
                WaitingInputUpdate();
                break;

                case TeleporState.PreTeleport:
                PreTeleportUpdate();
                break;

                case TeleporState.PostTeleport:
                PostTeleportUpdate();
                break;
            }
        }

        private void WaitingInputUpdate()
        {
            if (isTeleporting)
                return;

            //wait for the teleport to start
            if ( IsMovingJoystick(leftController) || IsMovingJoystick(rightController) )
            {
                UpdateActiveController();
                currentTeleportState = TeleporState.PreTeleport;
            }
        }

        private bool IsMovingJoystick(VR_Controller controller)
        {
            return controller.Input.GetJoystick().magnitude > 0.25f;
        }

        //set the controller that makes the teleport intent
        private void UpdateActiveController()
        {
            activeController = GetActiveController();
            lastActiveController = activeController;
        }

        private VR_Controller GetActiveController()
        {
            if (IsMovingJoystick(leftController) && IsMovingJoystick(rightController))
            {
                return lastActiveController == null ? rightController : lastActiveController;
            }

            if ( IsMovingJoystick(leftController) )
                return leftController;


            if (IsMovingJoystick(rightController))
                return rightController;

            return null;

        }

        //wait for the player to decide where to teleport
        private void PreTeleportUpdate()
        {
            UpdateActiveController();

            //there is no active controller try to do a teleport
            if (activeController == null)
            {
                //if we can teleport to the last AimRaycast
                if (IsAimRaycastInfoSuitableForTeleporting(lastRaycastInfo))
                {
                    DoTeleport( lastRaycastInfo );
                }

                //clean the line inmediatly
                teleportLineRender.CleanRender();

                //go to post teleport
                currentTeleportState = TeleporState.PostTeleport;
                return;
            }

            Ray controllerRay = new Ray( activeController.transform.position, activeController.transform.forward );

            //use the aimhandler to generate all the line points
            List<Vector3> points = aimHandler.GetAllPoints( controllerRay );
            //use the raycaster
            AimRaycastInfo info = aimRaycaster.Raycast( points , activeController.transform );

            if (info != null)
            {
                teleportLineRender.Render( info.validPoints, info.isValid );
            }
            else
            {
                teleportLineRender.Render( points , false );
            }           

            if (IsAimRaycastInfoSuitableForTeleporting( info ))
            {
                aimMarker.UpdatePositionAndRotation( activeController, info );
               
            }
            else
            {
                aimMarker.Hide();
            }

            lastRaycastInfo = info;
        }

        private bool IsAimRaycastInfoSuitableForTeleporting(AimRaycastInfo info)
        {
            return info != null && info.isValid;
        }

        private void DoTeleport(AimRaycastInfo info)
        {
            isTeleporting = true;
            teleportHandler.DoTeleport( characterController , aimMarker.Marker.transform , delegate { isTeleporting = false; } );
        }

        private void PostTeleportUpdate()
        {            
            aimMarker.Hide();
            lastActiveController = null;
            lastRaycastInfo = null;

            currentTeleportState = TeleporState.WaitingInput;
        }
       
    }

}

