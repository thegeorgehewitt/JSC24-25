using System;

using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.AI.Pathfinding;
using UnityEngine;

namespace Custom.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NavGrid2D))]
    public class NavGrid2DEditor : CustomBaseEditor
    {
        // Static Baking
        private SerializedProperty gridGenerateMode;
        private SerializedProperty blockableLayers;
        private SerializedProperty minAngle;
        private SerializedProperty maxAngle;
        private SerializedProperty tilemap;
        private SerializedProperty center;
        private SerializedProperty size;
        private SerializedProperty cellCount;

        // Dynamic Obstacles
        private SerializedProperty obstacleDetectBounds;
        private SerializedProperty obstacleUpdateMode;



        private AnimBool expandGridGeneration;
        private AnimBool expandDynamicObstacles;

        private bool IsGridGenerationExpanded
        {
            get { return SessionState.GetBool($"Grid Generation Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Grid Generation Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        private bool IsDynamicObstaclesExpanded
        {
            get { return SessionState.GetBool($"Dynamic Obstacles Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"Dynamic Obstacles Properties Expanded ({GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }



        private void OnEnable()
        {
            gridGenerateMode = AssignToProperty("gridGenerateMode");
            blockableLayers = AssignToProperty("blockableLayers");
            minAngle = AssignToProperty("minAngle");
            maxAngle = AssignToProperty("maxAngle");
            tilemap = AssignToProperty("tilemap");
            center = AssignToProperty("center");
            size = AssignToProperty("size");
            cellCount = AssignToProperty("cellCount");

            obstacleDetectBounds = AssignToProperty("obstacleDetectBounds");
            obstacleUpdateMode = AssignToProperty("obstacleUpdateMode");


            expandGridGeneration = new(IsGridGenerationExpanded);
            expandGridGeneration.valueChanged.AddListener(Repaint);

            expandDynamicObstacles = new(IsDynamicObstaclesExpanded);
            expandDynamicObstacles.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            #region Grid Generation
            IsGridGenerationExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsGridGenerationExpanded, "Grid Generation", CustomGUIStyles.foldoutHeader);
            expandGridGeneration.target = IsGridGenerationExpanded;
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (EditorGUILayout.BeginFadeGroup(expandGridGeneration.faded))
            {
                EditorGUILayout.Space();

                #region Layers
                EditorGUILayout.PropertyField(blockableLayers);

                if (blockableLayers.intValue == 0)
                {
                    EditorGUILayout.HelpBox(
                        "Blockable Layers is not set. All cells in nav grid will be defined as Empty",
                        MessageType.Warning);
                }
                #endregion

                EditorGUILayout.Space();

                #region Slope Angle Range
                GUIStyle rightLabel = new GUIStyle(EditorStyles.boldLabel);
                rightLabel.alignment = TextAnchor.UpperRight;

                float controlEndsWidth = 50f;
                float minAngleValue = minAngle.floatValue;
                float maxAngleValue = maxAngle.floatValue;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Slope Angle Range");

                EditorGUILayout.BeginVertical();
                minAngleValue = EditorGUILayout.FloatField(minAngleValue, GUILayout.Width(controlEndsWidth));
                EditorGUILayout.LabelField(
                    new GUIContent("Min", "Exclusive minimum angle value to be defined as a slope"), 
                    EditorStyles.boldLabel, 
                    GUILayout.Width(controlEndsWidth));
                EditorGUILayout.EndVertical();

                EditorGUILayout.MinMaxSlider(ref minAngleValue, ref maxAngleValue, 0f, 90f);

                EditorGUILayout.BeginVertical();
                maxAngleValue = EditorGUILayout.FloatField(maxAngleValue, GUILayout.Width(controlEndsWidth));
                EditorGUILayout.LabelField(
                    new GUIContent("Max", "Inclusive maximum angle value to be defined as a slope"), 
                    rightLabel, 
                    GUILayout.Width(controlEndsWidth));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                minAngle.floatValue = minAngleValue;
                maxAngle.floatValue = maxAngleValue;
                #endregion

                EditorGUILayout.Space();

                #region Grid Generation
                EditorGUILayout.PropertyField(gridGenerateMode);
                switch (gridGenerateMode.enumValueIndex)
                {
                    case (int)GridGenerationMode.Tilemap:
                        EditorGUILayout.PropertyField(tilemap);
                        break;

                    case (int)GridGenerationMode.FreeBounds:
                        EditorGUILayout.PropertyField(center);
                        EditorGUILayout.PropertyField(size);
                        EditorGUILayout.PropertyField(cellCount);
                        break;
                }
                #endregion

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            #region Dynamic Obstacle
            IsDynamicObstaclesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsDynamicObstaclesExpanded, "Dynamic Obstacle", CustomGUIStyles.foldoutHeader);
            expandDynamicObstacles.target = IsDynamicObstaclesExpanded;
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (EditorGUILayout.BeginFadeGroup(expandDynamicObstacles.faded))
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(obstacleDetectBounds);
                EditorGUILayout.PropertyField(obstacleUpdateMode);

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}
