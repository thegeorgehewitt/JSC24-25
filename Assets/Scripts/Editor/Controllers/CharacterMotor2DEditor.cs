using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.Controller;

namespace Custom.Editor
{
    using Styles;

    [CustomEditor(typeof(CharacterMotor2D), true)]
    [CanEditMultipleObjects]
    public class CharacterMotor2DEditor : UnityEditor.Editor
    {
        private CharacterMotor2D asTarget;

        private SerializedProperty rigidbody;
        private SerializedProperty animator;

        private SerializedProperty solidLayers;
        private SerializedProperty groundCheck;
        private SerializedProperty ceilingCheck;
        private SerializedProperty wallCheck;

        private SerializedProperty useGravity;
        private SerializedProperty fallAcceleration;
        private SerializedProperty maxFallSpeed;
        private SerializedProperty jumpEndEarlyGravityModifier;

        private SerializedProperty enableVisibilityCheck;
        private SerializedProperty lightEventListener;

        private SerializedProperty paused;
        private SerializedProperty controlScripts;

        private SerializedProperty velocity;

        private AnimBool expandProximityCheckProperties;
        private AnimBool expandGravityProperties;
        private AnimBool expandVisibilityProperties;
        private AnimBool expandControlsProperties;
        private AnimBool expandInfo;

        private bool IsProximityCheckPropertiesExpanded
        {
            get { return SessionState.GetBool($"Proximity Check Properties Expanded ({typeof(CharacterMotor2DEditor)}) : {serializedObject.targetObject.GetInstanceID()}", true); }
            set { SessionState.SetBool($"Proximity Check Properties Expanded ({typeof(CharacterMotor2DEditor)}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsGravityPropertiesExpanded
        {
            get { return SessionState.GetBool($"Gravity Properties Expanded ({typeof(CharacterMotor2DEditor)}) : {serializedObject.targetObject.GetInstanceID()}", true); }
            set { SessionState.SetBool($"Gravity Properties Expanded ({typeof(CharacterMotor2DEditor)}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsVisibilityPropertiesExpanded
        {
            get { return SessionState.GetBool($"Visibility Properties Expanded ({typeof(CharacterMotor2DEditor)}) : {serializedObject.targetObject.GetInstanceID()}", true); }
            set { SessionState.SetBool($"Visibility Properties Expanded ({typeof(CharacterMotor2DEditor)}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsControlsPropertiesExpanded
        {
            get { return SessionState.GetBool($"Controls Properties Expanded ({typeof(CharacterMotor2DEditor)}) : {serializedObject.targetObject.GetInstanceID()}", true); }
            set { SessionState.SetBool($"Controls Properties Expanded ({typeof(CharacterMotor2DEditor)}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsInfoExpanded
        {
            get { return SessionState.GetBool($"Info Expanded ({typeof(InteractableEnemyBaseEditor)}) : {serializedObject.targetObject.GetInstanceID()}", true); }
            set { SessionState.SetBool($"Info Expanded ({typeof(InteractableEnemyBaseEditor)}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }



        private void OnEnable()
        {
            asTarget = (CharacterMotor2D)target;

            SetupSerializedProperties();

            SetupAnimBools();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                $"This inspector is controlled by a custom editor.\n" +
                $"Edit this in {typeof(CharacterMotor2DEditor)} script.",
                MessageType.None);

            EditorGUILayout.Space(10);

            #region References
            EditorGUILayout.PropertyField(rigidbody);

            if (!rigidbody.objectReferenceValue)
            {
                EditorGUILayout.HelpBox(
                    "Missing Rigidbody2D reference.\n" +
                    "Motor will be disabled.",
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
            IsProximityCheckPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsProximityCheckPropertiesExpanded, "Proximity Checks", CustomEditorStyles.foldoutHeader);
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
                EditorGUILayout.Space(5);

                EditorGUILayout.PropertyField(groundCheck);
                EditorGUILayout.PropertyField(ceilingCheck);
                EditorGUILayout.PropertyField(wallCheck);

                EditorGUILayout.Space(10);
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            #region Gravity
            IsGravityPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsGravityPropertiesExpanded, "Gravity", CustomEditorStyles.foldoutHeader);
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
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            #region Visibility
            IsVisibilityPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsVisibilityPropertiesExpanded, "Visibility", CustomEditorStyles.foldoutHeader);
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
            IsControlsPropertiesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsControlsPropertiesExpanded, "Controls", CustomEditorStyles.foldoutHeader);
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
            IsInfoExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsInfoExpanded, "Info", CustomEditorStyles.foldoutHeader);
            expandInfo.target = IsInfoExpanded;
            EditorGUILayout.EndFoldoutHeaderGroup();

            GUI.enabled = false;
            if (EditorGUILayout.BeginFadeGroup(expandInfo.faded))
            {
                EditorGUILayout.PropertyField(velocity);
                EditorGUILayout.FloatField("Visibility", asTarget.Visibility);
                EditorGUILayout.Toggle("Is Grounded", asTarget.IsGrounded);
                EditorGUILayout.Toggle("Is On Ceiling", asTarget.IsOnCeiling);
                EditorGUILayout.Toggle("Is On Wall", asTarget.IsOnWall);

                EditorGUILayout.Space(10);
            }
            EditorGUILayout.EndFadeGroup();
            GUI.enabled = true;
            #endregion

            serializedObject.ApplyModifiedProperties();
        }



        private void SetupSerializedProperties()
        {
            rigidbody = serializedObject.FindProperty("rigidbody");
            animator = serializedObject.FindProperty("animator");

            solidLayers = serializedObject.FindProperty("solidLayers");
            groundCheck = serializedObject.FindProperty("groundCheck");
            ceilingCheck = serializedObject.FindProperty("ceilingCheck");
            wallCheck = serializedObject.FindProperty("wallCheck");

            useGravity = serializedObject.FindProperty("useGravity");
            fallAcceleration = serializedObject.FindProperty("fallAcceleration");
            maxFallSpeed = serializedObject.FindProperty("maxFallSpeed");
            jumpEndEarlyGravityModifier = serializedObject.FindProperty("jumpEndEarlyGravityModifier");

            enableVisibilityCheck = serializedObject.FindProperty("enableVisibilityCheck");
            lightEventListener = serializedObject.FindProperty("lightEventListener");

            paused = serializedObject.FindProperty("paused");
            controlScripts = serializedObject.FindProperty("controlScripts");

            velocity = serializedObject.FindProperty("velocity");
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