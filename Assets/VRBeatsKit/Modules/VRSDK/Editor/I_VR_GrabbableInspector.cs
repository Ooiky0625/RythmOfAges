using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Collections.Generic;
using Platinio.SDK.EditorTools;
using UnityEditor.EditorTools;

namespace VRSDK.EditorCode
{



    [CanEditMultipleObjects]
    [CustomEditor(typeof(VR_Grabbable))]
    public class I_VR_GrabbableInspector : Editor
    {

        private SerializedProperty onGrabStateChange = null;
        private SerializedProperty perfectGrab = null;
        private SerializedProperty grabDistance = null;
        private SerializedProperty grabFlyTime = null;
        private SerializedProperty shouldFly = null;
        private SerializedProperty startOnRightController = null;
        private SerializedProperty startOnLeftController = null;
        private SerializedProperty autoGrab = null;
        private SerializedProperty grabButton = null;
        private SerializedProperty grabLayer = null;
        private SerializedProperty unGrabLayer = null;
        private SerializedProperty enableColliderOnGrab = null;
        private SerializedProperty shareHandInteractSettings = null;
        private SerializedProperty shareHandAnimationSettings = null;
        private SerializedProperty leftHandSettings = null;
        private SerializedProperty handSettings = null;
        private SerializedProperty rightHandSettings = null;
        private SerializedProperty preserveKinematicState = null;
        private SerializedProperty useDistanceGrab = null;
        private SerializedProperty ignoreColliderList = null;
        private SerializedProperty toggleGrab = null;
        private SerializedProperty grabbableCollider = null;
        private SerializedProperty interactableType = null;
        
        private Texture handIcon = null;
        private Texture playIcon = null;
        private Texture eventIcon = null;
        private Texture settingsIcon = null;

        protected VR_Grabbable grabbable = null;
        protected VR_GrabbableEditorPart editorPart = null;

        protected List<ConsoleMessage> consoleList = null;

        
        protected virtual void OnEnable()
        {
            GetSerializeProperties();
            LoadIcons();

            grabbable = (VR_Grabbable) target;
            editorPart = grabbable.EditorPart;
        }

        private void GetSerializeProperties()
        {
            onGrabStateChange = serializedObject.FindProperty("onGrabStateChange");
            perfectGrab = serializedObject.FindProperty("perfectGrab");
            grabDistance = serializedObject.FindProperty("interactDistance");
            grabFlyTime = serializedObject.FindProperty("grabFlyTime");
            shouldFly = serializedObject.FindProperty("shouldFly");
            startOnRightController = serializedObject.FindProperty("startOnRightController");
            startOnLeftController = serializedObject.FindProperty("startOnLeftController");
            autoGrab = serializedObject.FindProperty("autoGrab");
            grabButton = serializedObject.FindProperty("interactButton");
            grabLayer = serializedObject.FindProperty("grabLayer");
            unGrabLayer = serializedObject.FindProperty("unGrabLayer");
            enableColliderOnGrab = serializedObject.FindProperty("enableColliderOnGrab");
            shareHandInteractSettings = serializedObject.FindProperty("shareHandInteractionSettings");
            shareHandAnimationSettings = serializedObject.FindProperty("shareHandAnimationSettings");
            rightHandSettings = serializedObject.FindProperty("rightHandSettings");
            leftHandSettings = serializedObject.FindProperty("leftHandSettings");
            handSettings = serializedObject.FindProperty("handSettings");
            preserveKinematicState = serializedObject.FindProperty("preserveKinematicState");
            useDistanceGrab = serializedObject.FindProperty("useDistanceGrab");
            ignoreColliderList = serializedObject.FindProperty("ignoreColliderList");
            toggleGrab = serializedObject.FindProperty("toggleGrab");
            interactableType = serializedObject.FindProperty("interactableType");
            grabbableCollider = serializedObject.FindProperty("grabCollider");

        }

        private void LoadIcons()
        {
            handIcon = PlatinioEditorGUILayout.LoadIcon("hand");
            playIcon = PlatinioEditorGUILayout.LoadIcon("play");
            eventIcon = PlatinioEditorGUILayout.LoadIcon("calendar");
            settingsIcon = PlatinioEditorGUILayout.LoadIcon("settings");
        }
       

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space(10);

            consoleList = new List<ConsoleMessage>();


            editorPart.selectedMenu = (GrabSelectionMenu) PlatinioEditorGUILayout.DrawGridButtons( (int) editorPart.selectedMenu, 2 , 
                new GUIContent(" Animation" , playIcon),
                new GUIContent(" Grab Settings", handIcon),                
                new GUIContent(" Other", settingsIcon),
                new GUIContent(" Events", eventIcon)
                );

            EditorGUILayout.Space(30);


            if (editorPart.selectedMenu == GrabSelectionMenu.Animation)
            {
                DrawGrabbableAnimationSettings();
            }
            else if (editorPart.selectedMenu == GrabSelectionMenu.GrabSettings)
            {
                DrawGrabSettings();
            }
            else if (editorPart.selectedMenu == GrabSelectionMenu.Other)
            {
                DrawOtherSettings();
            }
            else if (editorPart.selectedMenu == GrabSelectionMenu.Events)
            {
                DrawEventSettings();
            }

            ConsoleUpdate();

            EditorGUILayout.Space(20);

            GUIContent content = new GUIContent("Console (" + consoleList.Count + ")", "");
            PlatinioEditorGUILayout.FoldoutInspector(content, ref editorPart.foldoutConsole , delegate { DrawConsole(); });

            EditorGUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();

        }

        private void DrawConsole()
        {
            PlatinioEditorGUILayout.DrawConsole(consoleList);
        }

        private void ConsoleUpdate()
        {
            AnimationSettingsConsoleUpdate();
            GrabSettingsConsoleUpdate();
        }

        private void DrawGrabbableAnimationSettings()
        {
            DrawGrabbableAnimationSettingsHeader();
            EditorGUILayout.Space(20);
            DrawGrabbableAnimationSettingsBody();
        }

        protected virtual void DrawGrabbableAnimationSettingsHeader()
        {
            PlatinioEditorGUILayout.DrawTooltipBox(playIcon, "Animation Settings",
                "You can define diferent grab animations for each hand, and you can leave it empty if you don't wanna play any animation," +
                " some people prefer to hide the hand while grabbing something too.");      
        }

        protected virtual void DrawGrabbableAnimationSettingsBody()
        {            
            SerializedProperty rightHand = serializedObject.FindProperty("rightHandAnimationSettings");
            SerializedProperty leftHand = serializedObject.FindProperty("leftHandAnimationSettings");
            SerializedProperty share = serializedObject.FindProperty("handAnimationSettings");


            GUIContent content = new GUIContent("Share Settings?", "Use the same settings for the left and right hand?");
            shareHandAnimationSettings.boolValue = PlatinioEditorGUILayout.DrawBoolEnum(content, shareHandAnimationSettings.boolValue);

            if (shareHandAnimationSettings.boolValue)
            {
                content = new GUIContent("Both Hands", "Share settings for both hands");
                PlatinioEditorGUILayout.FoldoutInspector(content, ref editorPart.foldoutShareHandAnimationSettings, delegate { DrawHandAnimationSettings(share); });
            }
            else
            {
                content = new GUIContent("Left Hand", "Left hand animation settings");
                PlatinioEditorGUILayout.FoldoutInspector(content, ref editorPart.foldoutRightHandAnimationSettings, delegate { DrawHandAnimationSettings(leftHand); });

                EditorGUILayout.Space();

                content = new GUIContent("Right Hand", "Right hand animation settings");
                PlatinioEditorGUILayout.FoldoutInspector(content, ref editorPart.foldoutLeftHandAnimationSettings, delegate { DrawHandAnimationSettings(rightHand); });
            }
        }

        private void AnimationSettingsConsoleUpdate()
        {
            VR_HandAnimationSettings leftHand = grabbable.LeftHandAnimationSettings;
            VR_HandAnimationSettings rightHand = grabbable.RightHandAnimationSettings;
            VR_HandAnimationSettings share = grabbable.HandAnimationSettings;

            ConsoleMessage animationEmptyLog = new ConsoleMessage("You have missing grab animations, default grab animation will be use", MessageType.Warning);

            if (shareHandAnimationSettings.boolValue)
            {
                if (!share.hideHandOnGrab && share.animation == null)
                    consoleList.Add(animationEmptyLog);
            }
            else
            {
                if (!leftHand.hideHandOnGrab && leftHand.animation == null)
                    consoleList.Add(animationEmptyLog);
                else if (!rightHand.hideHandOnGrab && rightHand.animation == null)
                    consoleList.Add(animationEmptyLog);
            }
        }

        private void GrabSettingsConsoleUpdate()
        {
            VR_HandInteractSettings leftHand = grabbable.LeftHandSettings;
            VR_HandInteractSettings rightHand = grabbable.RightHandSettings;
            VR_HandInteractSettings share = grabbable.HandSettings;


            ConsoleMessage grabPointMissing = new ConsoleMessage("You forget to assign a grab point in GrabSettings", MessageType.Error);
            ConsoleMessage highlightPointMissing = new ConsoleMessage("You forget to assign a higlight point in GrabSettings", MessageType.Warning);

            if (!shareHandInteractSettings.boolValue)
            {
                if ( editorPart.IsMissingGrabPoint( rightHand ) || editorPart.IsMissingGrabPoint(leftHand ))
                    consoleList.Add(grabPointMissing);
                if ( editorPart.IsMissinHiglightPoint(rightHand) || editorPart.IsMissinHiglightPoint(leftHand) )
                    consoleList.Add(highlightPointMissing);
            }
            else
            {
                Debug.Log("share hand interact settings value " + shareHandAnimationSettings.boolValue);

                if ( editorPart.IsMissingGrabPoint(share) )
                    consoleList.Add(grabPointMissing);
                if (editorPart.IsMissinHiglightPoint(share))
                    consoleList.Add(highlightPointMissing);
            }

        }

       
        private void DrawHandAnimationSettings(SerializedProperty property)
        {
            SerializedProperty hideOnGrab = property.FindPropertyRelative("hideHandOnGrab");
            SerializedProperty animation = property.FindPropertyRelative("animation");

            EditorGUILayout.BeginVertical("Box");
            GUIContent content = new GUIContent("Hide Hand?", "Should this hand hide while grabbing the object?");            
            hideOnGrab.boolValue = PlatinioEditorGUILayout.DrawBoolEnum(content , hideOnGrab.boolValue);

            if (!hideOnGrab.boolValue)
            {
                content = new GUIContent("Grab Animation", "The animation use while grabbing the object");               
                EditorGUILayout.PropertyField(animation , content);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawGrabSettings()
        {
            DrawGrabSettingsHeader();
            PlatinioEditorGUILayout.Space(3);
            DrawGrabSettingsBody();
        }

        protected virtual void DrawGrabSettingsHeader()
        {
            PlatinioEditorGUILayout.DrawTooltipBox(handIcon, "Grab Settings",
               "In this section you can define the behavior of the grabbable object, for example should this object move to the hand position? use " +
               "toggle or use auto grab feature");
        }

        protected virtual void DrawGrabSettingsBody()
        {
            GUIContent content = new GUIContent("Basic", "Grabbable basic settings");
            PlatinioEditorGUILayout.FoldoutInspector(content, ref editorPart.foldoutBasic, DrawGrabBasicSettings);

            PlatinioEditorGUILayout.Space(2);

            content = new GUIContent("Interaction", "Grabbable interaction settings");
            PlatinioEditorGUILayout.FoldoutInspector(content, ref editorPart.foldoutInteraction, DrawHandInteractionSettings);

            PlatinioEditorGUILayout.Space(2);

            content = new GUIContent("Input", "Grabbable input settings");
            PlatinioEditorGUILayout.FoldoutInspector(content, ref editorPart.foldoutInput, DrawGrabInput);
            
            content = new GUIContent("Editor Tools", "");
            PlatinioEditorGUILayout.FoldoutInspector(content, ref editorPart.foldoutEditorTools, DrawEditorTools);
        }

        private void DrawEditorTools()
        {
            EditorGUILayout.BeginVertical("Box");
            if (GUILayout.Button("Update Grab Position Offset"))
            {
                grabbable.UpdateGrabPositionOffset();
            }
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawGrabInput()
        {
            EditorGUILayout.BeginVertical("Box");

            GUIContent content = new GUIContent("Use Auto Grab", "Button use in controller for grabbing");
            autoGrab.boolValue = PlatinioEditorGUILayout.DrawBoolEnum(content , autoGrab.boolValue);

            if (!autoGrab.boolValue)
            {
                grabbable.SetStartOnLeftHand(false);
                grabbable.SetStartOnRightHand(false);

                content = new GUIContent("Grab Button", "Button use in controller for grabbing");
                grabButton.enumValueIndex = (int)(VR_InputButton)EditorGUILayout.EnumPopup(content, (VR_InputButton) grabButton.enumValueIndex);

                content = new GUIContent("Use Toggle", "Button use in controller for grabbing");
                toggleGrab.boolValue = PlatinioEditorGUILayout.DrawBoolEnum(content , toggleGrab.boolValue);
            }
            else
            {

                int newSelection = startOnLeftController.boolValue ? 1 : 0;
                newSelection = PlatinioEditorGUILayout.DrawGridButtons(newSelection, 2,
                   new GUIContent(" Right Hand"),
                   new GUIContent(" Left Hand")

                   );

                startOnLeftController.boolValue = newSelection == 1;
                startOnRightController.boolValue = newSelection == 0;
                editorPart.handSelected = newSelection;

            }


            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawGrabBasicSettings()
        {
            EditorGUILayout.BeginVertical("Box");

            GUIContent content = new GUIContent("Use Distance Grab?", "Enable this object to be grabbed by the user very far away " +
                "by just pointing the hand to the object and pressing the grab button");
            useDistanceGrab.boolValue = PlatinioEditorGUILayout.DrawBoolEnum(content, useDistanceGrab.boolValue);

            content = new GUIContent("Grab Distance", "How far the hand can be it order to grab the object");
            grabDistance.floatValue = EditorGUILayout.FloatField(content, grabDistance.floatValue);

            content = new GUIContent("Update Grab Point?", "If in use the object grab point will be update base on the hand position");
            perfectGrab.boolValue = PlatinioEditorGUILayout.DrawBoolEnum(content , perfectGrab.boolValue);

            if (!perfectGrab.boolValue)
            {
                content = new GUIContent("Use Fly?", "Should this object fly to the hand?");
                shouldFly.boolValue = PlatinioEditorGUILayout.DrawBoolEnum(content , shouldFly.boolValue);

                if (shouldFly.boolValue)
                {
                    content = new GUIContent("Fly Time", "time in seconds use in order to fly to the hand");
                    grabFlyTime.floatValue = EditorGUILayout.FloatField(content, grabFlyTime.floatValue);
                }
            }
            
            content = new GUIContent("Interactable Type", "The type of interactable type (Recommended Distance)");
            interactableType.enumValueIndex = (int)(InteractableType)EditorGUILayout.EnumPopup(content, (InteractableType) interactableType.enumValueIndex);

            if (interactableType.enumValueIndex == (int) InteractableType.Collider)
            {
                content = new GUIContent("Grabbable Collider", "Collider use to detect interactions");
                grabbableCollider.objectReferenceValue = EditorGUILayout.ObjectField(content, grabbableCollider.objectReferenceValue, typeof(Collider), true);

                if (grabbableCollider.objectReferenceValue == null)
                {
                    ConsoleMessage colliderMissing = new ConsoleMessage("You forget to assign an interact collider in Grab Settings", MessageType.Error);
                    consoleList.Add(colliderMissing);
                }
            }

            EditorGUILayout.EndVertical();

        }

        protected virtual void DrawHandInteractionSettings()
        {
            EditorGUILayout.BeginVertical("Box");
            
            SerializedProperty share = serializedObject.FindProperty("handSettings");
            SerializedProperty leftHand = serializedObject.FindProperty("leftHandSettings");
            SerializedProperty rightHand = serializedObject.FindProperty("rightHandSettings");


            GUIContent content = new GUIContent("Share Settings?", "Use the same settings for the left and right hand?");
            shareHandInteractSettings.boolValue = PlatinioEditorGUILayout.DrawBoolEnum(content , shareHandInteractSettings.boolValue);


            if (shareHandInteractSettings.boolValue)
            {
                content = new GUIContent("Both Hands", "Share settings for both hands");
                PlatinioEditorGUILayout.FoldoutInspector(content, ref editorPart.foldoutShareHandInteractSettings, 1, delegate { DrawHandInteractionInspector(share); });
            }
            else
            {
                content = new GUIContent("Right Hand", "Settings apply just to the right hand");
                PlatinioEditorGUILayout.FoldoutInspector(content, ref editorPart.foldoutRightHandInteractSettings, 1, delegate { DrawHandInteractionInspector(rightHand); });

                content = new GUIContent("Left Hand", "Settings apply just to the left hand");
                PlatinioEditorGUILayout.FoldoutInspector(content, ref editorPart.foldoutLeftHandInteractSettings, 1, delegate { DrawHandInteractionInspector(leftHand); });
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawHandInteractionInspector(SerializedProperty property)
        {
            GUIContent content;
            Rect rect;

            SerializedProperty canInteract = property.FindPropertyRelative("canInteract");
            SerializedProperty interactPoint = property.FindPropertyRelative("interactPoint");
            SerializedProperty higlightPoint = property.FindPropertyRelative("highlightPoint");
            SerializedProperty rotationOffset = property.FindPropertyRelative("rotationOffset");

            if (!shareHandInteractSettings.boolValue)
            {
                content = new GUIContent("Can Grab?", "Can this hand grab the object?");
                rect = GUILayoutUtility.GetRect(40f, 60f, 16f, 16f);
                rect.width -= 25;
                rect.x += 25;
               
                canInteract.boolValue = PlatinioEditorGUILayout.DrawBoolEnum(content , rect , canInteract.boolValue);
                EditorGUILayout.Space(3);
            }
            else 
            {
                property.FindPropertyRelative("canInteract").boolValue = true;
            }
            

            if (canInteract.boolValue)
            {
                content = new GUIContent("Grab Point", "The point from where this object can be grabbed");
                rect = GUILayoutUtility.GetRect(40f, 60f, 16f, 16f);
                rect.width -= 25;
                rect.x += 25;
               
                EditorGUI.PropertyField(rect, interactPoint, content);
                EditorGUILayout.Space(3);

                content = new GUIContent("Highlight Point", "The point from where this object start the higlight");
                rect = GUILayoutUtility.GetRect(40f, 60f, 16f, 16f);
                rect.width -= 25;
                rect.x += 25;
                
                EditorGUI.PropertyField(rect, higlightPoint, content);

                content = new GUIContent("Rotation offset", "The rotation offset used while the object is being grab");
                rect = GUILayoutUtility.GetRect(80f, 80f, 45f, 45f);
                rect.width -= 25;
                rect.x += 25;
               
                EditorGUI.PropertyField(rect , rotationOffset , content);
            }


        }

        private void DrawOtherSettings()
        {
            DrawOtherSettingsHeader();
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical("Box");
            DrawOtherSettingsBody();
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawOtherSettingsHeader()
        {
            PlatinioEditorGUILayout.DrawTooltipBox(settingsIcon, "Other Settings",
               "In this section you have no so common settings but still useful in some cases");
        }

        protected virtual void DrawOtherSettingsBody()
        {
            

            GUIContent content = new GUIContent("Preserve Kinematic? ", "The normal behaviour in a grabbable item is override the kinematic state " +
                "of the object so I can respond to physics once it is dropped, use this if you wanna to preserve the kinematic state of an object once it is dropped");
            preserveKinematicState.boolValue = PlatinioEditorGUILayout.DrawBoolEnum(content, preserveKinematicState.boolValue);

            content = new GUIContent("Use Collider? ", "Use collider while grabbing?");
            enableColliderOnGrab.boolValue = PlatinioEditorGUILayout.DrawBoolEnum(content, enableColliderOnGrab.boolValue);

            content = new GUIContent("Grab Layer ", "Collision layer used while this object is in grab state");
            grabLayer.intValue = EditorGUILayout.LayerField(content, grabLayer.intValue);

            content = new GUIContent("UnGrab Layer ", "Collision layer used while this object is ungrab state");
            unGrabLayer.intValue = EditorGUILayout.LayerField(content, unGrabLayer.intValue);

            PlatinioEditorGUILayout.PropertyField(ignoreColliderList, true);

           
        }


        private void DrawEventSettings()
        {

            DrawEventSettingsHeader();
            EditorGUILayout.Space(10);
            DrawEventSettingsBody();

        }

        protected virtual void DrawEventSettingsHeader()
        {
            PlatinioEditorGUILayout.DrawTooltipBox(eventIcon, "Events",
               "You can control the logic from your grabbables in your game mostly by using this event, it sends an enum called" +
               "GrabState, and every value define a diferent action made by the user.\n\n" +
               "<b>Grab:</b> called the first frame when the player try to grab this object.\n" +
               "<b>Drop:</b> called the first frame when the player drops this object.\n" +
               "<b>UnGrab:</b> called inmediatly after drop, and stay in UnGrab state until grab event is raise again\n");
        }

        protected virtual void DrawEventSettingsBody()
        {            
            EditorGUILayout.BeginVertical("Box");
            PlatinioEditorGUILayout.PropertyField(onGrabStateChange);
            EditorGUILayout.EndVertical();
        }


        public void OnSceneGUI()
        {

            float newGrabDistance = grabbable.GrabDistance;
            Transform center = null;
           
            if (shareHandInteractSettings.boolValue)
            {
                center = grabbable.HandSettings.interactPoint;
            }
            else
            {
                center = grabbable.RightHandSettings.interactPoint == null ? grabbable.LeftHandSettings.interactPoint : grabbable.RightHandSettings.interactPoint;
            }

            if (center == null)
                center = grabbable.transform;

            newGrabDistance = DrawDiscRadiusHandler(center.position, newGrabDistance);

            grabbable.SetInteractDistanceViaInspector(newGrabDistance);
            EditorUtility.SetDirty(grabbable);

        }

        private float DrawDiscRadiusHandler(Vector3 center , float radius)
        {
            if (Camera.current == null)
                return radius;

            Handles.color = Color.blue;
            Handles.color = new Color(0.0f, 0.0f, 1.0f, 0.125f);
            Handles.DrawSolidDisc(center, Camera.current.transform.forward, radius);
            Handles.color = new Color(0.0f, 0.0f, 1.0f, 1.0f);

            return Handles.RadiusHandle(Quaternion.LookRotation(Camera.current.transform.forward * -1.0f), center, radius, true);      
            
        }

       
    }

}

