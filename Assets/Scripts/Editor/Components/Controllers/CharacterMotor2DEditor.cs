using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.Controller;

namespace Custom.Editor
{
    [CustomEditor(typeof(CharacterMotor2D), true)]
    [CanEditMultipleObjects]
    public class CharacterMotor2DEditor : CustomBaseEditor
    {
        private CharacterMotor2D asTarget;

        private SerializedProperty rigidbody;
        private SerializedProperty animator;
        private SerializedProperty capsuleCollider;

        private SerializedProperty solidLayers;
        private SerializedProperty groundCheck;
        private SerializedProperty ceilingCheck;
        private SerializedProperty wallCheck;
        private SerializedProperty footSocket;
        private SerializedProperty headSocket;
        private SerializedProperty frontSocket;

        private SerializedProperty useGravity;
        private SerializedProperty fallAcceleration;
        private SerializedProperty maxFallSpeed;
        private SerializedProperty jumpEndEarlyGravityModifier;
        private SerializedProperty slopeHandler;

        private SerializedProperty enableVisibilityCheck;
        private SerializedProperty lightEventListener;

        private SerializedProperty paused;
        private SerializedProperty inputGroupMode;
        private SerializedProperty controlScripts;

        private SerializedProperty velocity;

        private AnimBool expandProximityCheckProperties;
        private AnimBool expandGravityProperties;
        private AnimBool expandVisibilityProperties;
        private AnimBool expandControlsProperties;
        private AnimBool expandInfo;

        private bool IsProximityCheckPropertiesExpanded
        {
            get { return SessionState.GetBool($"Proximity Check Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Proximity Check Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsGravityPropertiesExpanded
        {
            get { return SessionState.GetBool($"Gravity Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Gravity Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsVisibilityPropertiesExpanded
        {
            get { return SessionState.GetBool($"Visibility Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Visibility Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsControlsPropertiesExpanded
        {
            get { return SessionState.GetBool($"Controls Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Controls Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsInfoExpanded
        {
            get { return SessionState.GetBool($"Info Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Info Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }



        private void OnEnable()
        {
            asTarget = (CharacterMotor2D)target;

            SetupSerializedProperties();

            SetupAnimBools();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            #region References
            EditorGUILayout.PropertyField(rigidbody);
            if (!rigidbody.objectReferenceValue)
            {
                EditorGUILayout.HelpBox(
                    $"Missing {rigidbody.type} reference.\n" +
                    $"Motor will be disabled.",
                    MessageType.Error);
            }

            EditorGUILayout.PropertyField(capsuleCollider);
            if (!capsuleCollider.objectReferenceValue)
            {
                EditorGUILayout.HelpBox(
                    $"Missing {capsuleCollider.type} reference.\n" +
                    $"Motor will be disabled.",
                    MessageType.Error);
            }

            EditorGUILayout.Space(10);
            
            EditorGUILayout.PropertyField(animator);

            if (!animator.objectReferenceValue)
            {
                EditorGUILayout.HelpBox(
                    "Missing Animator reference.\n" +
                    "Animations will be disabled.",
                    MessageType.Error);
            }

            EditorGUILayout.Space(10);
            #endregion

            #region Proximity Check
            IsProximityCheckPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsProximityCheckPropertiesExpanded, "Proximity Checks", CustomGUIStyles.foldoutHeader);
            expandProximityCheckProperties.target = IsProximityCheckPropertiesExpanded;
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (EditorGUILayout.BeginFadeGroup(expandProximityCheckProperties.faded))
            {
                EditorGUILayout.PropertyField(solidLayers);

                if (solidLayers.intValue == 0)
                {
                    EditorGUILayout.HelpBox(
                        "Solid Layers is not set. Proximity checks will be ignored.",
                        MessageType.Warning);
                }
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(groundCheck);
                EditorGUILayout.PropertyField(ceilingCheck);
                EditorGUILayout.PropertyField(wallCheck);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(footSocket);
                EditorGUILayout.PropertyField(headSocket);
                EditorGUILayout.PropertyField(frontSocket);

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            #region Gravity
            IsGravityPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsGravityPropertiesExpanded, "Gravity", CustomGUIStyles.foldoutHeader);
            expandGravityProperties.target = IsGravityPropertiesExpanded;

            if (EditorGUILayout.BeginFadeGroup(expandGravityProperties.faded))
            {
                EditorGUILayout.PropertyField(useGravity);

                GUI.enabled = useGravity.boolValue;
                {
                    EditorGUILayout.PropertyField(fallAcceleration);
                    EditorGUILayout.PropertyField(maxFallSpeed);
                    EditorGUILayout.PropertyField(jumpEndEarlyGravityModifier);
                }
                GUI.enabled = true;

                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(slopeHandler);

                EditorGUILayout.Space(10);
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            #region Visibility
            IsVisibilityPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsVisibilityPropertiesExpanded, "Visibility", CustomGUIStyles.foldoutHeader);
            expandVisibilityProperties.target = IsVisibilityPropertiesExpanded;
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (EditorGUILayout.BeginFadeGroup(expandVisibilityProperties.faded))
            {
                EditorGUILayout.PropertyField(enableVisibilityCheck);

                GUI.enabled = enableVisibilityCheck.boolValue;
                {
                    EditorGUILayout.PropertyField(lightEventListener);
                    if (lightEventListener.objectReferenceValue == null && enableVisibilityCheck.boolValue)
                    {
                        EditorGUILayout.HelpBox(
                            "Missing reference of type LightEventListener. Visibility will be set to default of 1.",
                            MessageType.Warning);
                    }
                }
                GUI.enabled = true;


                EditorGUILayout.Space(10);
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            #region Controls
            IsControlsPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsControlsPropertiesExpanded, "Controls", CustomGUIStyles.foldoutHeader);
            expandControlsProperties.target = IsControlsPropertiesExpanded;
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (EditorGUILayout.BeginFadeGroup(expandControlsProperties.faded))
            {
                EditorGUILayout.PropertyField(paused);
                if (paused.boolValue)
                {
                    EditorGUILayout.HelpBox(
                        "Controls are paused.\n" +
                        "Inputs will be ignored and control scripts will no longer be updated.",
                        MessageType.Info);
                }

                EditorGUILayout.PropertyField(inputGroupMode);
                EditorGUILayout.PropertyField(controlScripts);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                bool pressed = GUILayout.Button("Get Control Scripts in Children", GUI.skin.button, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                if (pressed)
                {
                    controlScripts.ClearArray();
                    foreach (var script in asTarget.GetComponentsInChildren<CharacterControlBase>())
                    {
                        controlScripts.InsertArrayElementAtIndex(0);
                        controlScripts.GetArrayElementAtIndex(0).objectReferenceValue = script;
                    }
                }

                EditorGUILayout.Space(10);
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            #region Info
            IsInfoExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsInfoExpanded, "Info", CustomGUIStyles.foldoutHeader);
            expandInfo.target = IsInfoExpanded;
            EditorGUILayout.EndFoldoutHeaderGroup();

            GUI.enabled = false;
            if (EditorGUILayout.BeginFadeGroup(expandInfo.faded))
            {
                EditorGUILayout.PropertyField(velocity);
                EditorGUILayout.FloatField("Visibility", asTarget.Visibility);
                EditorGUILayout.Toggle("Is Grounded", asTarget.IsGrounded);
                EditorGUILayout.Toggle("Is On Ceiling", asTarget.IsOnCeiling);
                EditorGUILayout.Toggle("Is Near Wall", asTarget.IsNearWall);

                EditorGUILayout.Space(10);
            }
            EditorGUILayout.EndFadeGroup();
            GUI.enabled = true;
            #endregion

            serializedObject.ApplyModifiedProperties();
        }



        private void SetupSerializedProperties()
        {
            rigidbody = AssignToProperty("rigidbody");
            capsuleCollider = AssignToProperty("capsuleCollider");
            animator = AssignToProperty("animator");

            solidLayers = AssignToProperty("solidLayers");
            groundCheck = AssignToProperty("groundCheck");
            ceilingCheck = AssignToProperty("ceilingCheck");
            wallCheck = AssignToProperty("wallCheck");
            footSocket = AssignToProperty("footSocket");
            headSocket = AssignToProperty("headSocket");
            frontSocket = AssignToProperty("frontSocket");

            useGravity = AssignToProperty("useGravity");
            fallAcceleration = AssignToProperty("fallAcceleration");
            maxFallSpeed = AssignToProperty("maxFallSpeed");
            jumpEndEarlyGravityModifier = AssignToProperty("jumpEndEarlyGravityModifier");
            slopeHandler = AssignToProperty("slopeHandler");

            enableVisibilityCheck = AssignToProperty("enableVisibilityCheck");
            lightEventListener = AssignToProperty("lightEventListener");

            paused = AssignToProperty("paused");
            inputGroupMode = AssignToProperty("inputGroupMode");
            controlScripts = AssignToProperty("controlScripts");

            velocity = AssignToProperty("velocity");
        }

        private void SetupAnimBools()
        {
            expandProximityCheckProperties = new();
            expandProximityCheckProperties.valueChanged.AddListener(Repaint);

            expandGravityProperties = new();
            expandGravityProperties.valueChanged.AddListener(Repaint);

            expandVisibilityProperties = new();
            expandVisibilityProperties.valueChanged.AddListener(Repaint);

            expandControlsProperties = new();
            expandControlsProperties.valueChanged.AddListener(Repaint);

            expandInfo = new();
            expandInfo.valueChanged.AddListener(Repaint);

        }
    }
}