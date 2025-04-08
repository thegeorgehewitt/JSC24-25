using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.Interactable.Character;
using Custom.Utility;
using Custom.AI.Pathfinding;

namespace Custom.Editor
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractableCharacterBase), true)]
    public class InteractableCharacterBaseEditor : InteractableObjectEditor
    {
        private InteractableCharacterBase asCharacter;

        private SerializedProperty detectionType;
        private SerializedProperty trackableLayers;
        private SerializedProperty visionBlockFilter;

        private SerializedProperty enableProximity;
        private SerializedProperty useBlockFilter;
        private SerializedProperty proximityDetectRange;

        private SerializedProperty enableFieldOfView;
        private SerializedProperty radius;
        private SerializedProperty angle;
        private SerializedProperty localRotation;
        private SerializedProperty FOVDisplay;

        private SerializedProperty behaviourTree;
        private SerializedProperty navAgent;


        private AnimBool expandEnemyProperties;
        private AnimBool showTargetDetectionProperties;

        private bool IsExpanded
        {
            get => SessionState.GetBool($"Expanded {typeof(InteractableCharacterBaseEditor)} : {serializedObject.targetObject.GetInstanceID()}", false);
            set => SessionState.SetBool($"Expanded {typeof(InteractableCharacterBaseEditor)} : {serializedObject.targetObject.GetInstanceID()}", value);
        }



        private void InitProperties()
        {
            detectionType = AssignToProperty("detectionType");
            trackableLayers = AssignToProperty("trackableLayers");
            visionBlockFilter = AssignToProperty("visionBlockFilter");

            enableProximity = AssignToProperty("enableProximity");
            useBlockFilter = AssignToProperty("useBlockFilter");
            proximityDetectRange = AssignToProperty("proximityDetectRange");

            enableFieldOfView = AssignToProperty("enableFieldOfView");
            radius = AssignToProperty("radius");
            angle = AssignToProperty("angle");
            localRotation = AssignToProperty("localRotation");
            FOVDisplay = AssignToProperty("FOVDisplay");

            navAgent = AssignToProperty("navAgent");
            behaviourTree = AssignToProperty("behaviourTree");
        }

        private void InitAnimValues()
        {
            expandEnemyProperties = new(IsExpanded);
            expandEnemyProperties.valueChanged.AddListener(Repaint);

            showTargetDetectionProperties = new(FOVDisplay.objectReferenceValue != null);
            showTargetDetectionProperties.valueChanged.AddListener(Repaint);
        }



        protected override void OnEnable()
        {
            base.OnEnable();

            asCharacter = (InteractableCharacterBase)target;

            InitProperties();
            InitAnimValues();
        }

        protected virtual void OnSceneGUI()
        {
            Vector3 position = asCharacter.transform.position;
            ViewCone viewCone = asCharacter.GetViewCone();
            float constantSize = HandleUtility.GetHandleSize(position);

            #region Proximity
            if (enableProximity.boolValue)
            {
                GUIStyle proximityLabel = new GUIStyle();
                proximityLabel.alignment = TextAnchor.MiddleCenter;
                proximityLabel.normal.textColor = Color.yellow;

                Handles.color = Color.yellow;

                Handles.DrawWireArc(position, Vector3.back, Vector3.up, 360, proximityDetectRange.floatValue);
                Handles.Label(position + Vector3.up * (proximityDetectRange.floatValue + constantSize * 0.2f), "Proximity Check Area", proximityLabel);
            }
            #endregion

            #region FOV 
            if (enableFieldOfView.boolValue)
            {
                GUIStyle FOVLabel = new GUIStyle();
                FOVLabel.alignment = TextAnchor.MiddleLeft;
                FOVLabel.normal.textColor = Color.cyan;

                Handles.color = Color.cyan;

                Vector3 viewAngleFrom = FieldOfView.DirectionFromAngle(viewCone, asCharacter.transform, -viewCone.Angle / 2);
                Vector3 viewAngleTo = FieldOfView.DirectionFromAngle(viewCone, asCharacter.transform, viewCone.Angle / 2);
                Vector3 directAngle = FieldOfView.DirectionFromAngle(viewCone, asCharacter.transform, 0);

                Handles.DrawWireArc(viewCone.Origin, Vector3.back, viewAngleFrom, viewCone.Angle, viewCone.Radius);
                Handles.DrawLine(viewCone.Origin, viewCone.Origin + viewAngleFrom * viewCone.Radius);
                Handles.DrawLine(viewCone.Origin, viewCone.Origin + viewAngleTo * viewCone.Radius);

                Handles.Label(viewCone.Origin + directAngle * (viewCone.Radius - constantSize * 0.2f), "FOV Check Area", FOVLabel);
            }
            #endregion
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            #region Foldout
            IsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsExpanded, "Interactable Character Properties", CustomGUIStyles.foldoutHeader);
            EditorGUILayout.EndFoldoutHeaderGroup();

            expandEnemyProperties.target = IsExpanded;

            if (EditorGUILayout.BeginFadeGroup(expandEnemyProperties.faded))
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                #region Target Detection
                EditorGUILayout.PropertyField(detectionType);

                EditorGUILayout.PropertyField(trackableLayers);
                if (trackableLayers.intValue == 0)
                {
                    EditorGUILayout.HelpBox(
                        "Tracking Layers is not set.\n" +
                        "This character will not be able to detect objects.",
                        MessageType.Warning);
                }

                EditorGUILayout.PropertyField(visionBlockFilter);
                #endregion

                #region Proximity
                EditorGUILayout.PropertyField(enableProximity);

                if (enableProximity.boolValue)
                {
                    EditorGUILayout.PropertyField(useBlockFilter);
                    EditorGUILayout.PropertyField(proximityDetectRange);
                    proximityDetectRange.floatValue = Mathf.Max(proximityDetectRange.floatValue, 0);
                }
                #endregion

                #region Field Of VIew
                EditorGUILayout.PropertyField(enableFieldOfView);

                if (enableFieldOfView.boolValue)
                {
                    EditorGUILayout.PropertyField(radius);
                    EditorGUILayout.PropertyField(angle);
                    EditorGUILayout.PropertyField(localRotation);

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(FOVDisplay);
                    if (!FOVDisplay.objectReferenceValue)
                    {
                        EditorGUILayout.HelpBox(
                            $"Assign a {typeof(FieldOfViewDisplay)} to view accurate FOV display.",
                            MessageType.Info);
                    }
                }
                #endregion

                #region Behaviour
                EditorGUILayout.PropertyField(behaviourTree);
                EditorGUILayout.PropertyField(navAgent);

                if (!navAgent.objectReferenceValue)
                {
                    EditorGUILayout.HelpBox(
                        $"A character with no {typeof(NavGridAgentBase)} attached is considered stationary.",
                        MessageType.Info); 
                }
                #endregion

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}
