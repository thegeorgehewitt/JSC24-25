using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.Utility;

namespace Custom.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : UnityEditor.Editor
    {
        private FieldOfView asTarget;

        private AnimBool meshDisplayGroupVisible;
        private SerializedProperty drawViewMeshProperty;
        private SerializedProperty meshResolutionProperty;
        private SerializedProperty edgeDistanceThresholdProperty;
        private SerializedProperty edgeResolveIterationsProperty;

        private AnimBool previewGroupVisible;
        private SerializedProperty previewProperty;
        private SerializedProperty handlesColorProperty;

        private void OnEnable()
        {
            asTarget = (FieldOfView)target;

            drawViewMeshProperty = serializedObject.FindProperty("drawViewMesh");
            meshResolutionProperty = serializedObject.FindProperty("meshResolution");
            edgeDistanceThresholdProperty = serializedObject.FindProperty("edgeDistanceThreshold");
            edgeResolveIterationsProperty = serializedObject.FindProperty("edgeResolveIterations");

            previewProperty = serializedObject.FindProperty("preview");
            handlesColorProperty = serializedObject.FindProperty("handlesColor");

            previewGroupVisible = new(asTarget.preview);
            previewGroupVisible.valueChanged.AddListener(Repaint);

            meshDisplayGroupVisible = new(asTarget.drawViewMesh);
            meshDisplayGroupVisible.valueChanged.AddListener(Repaint);
        }

        private void OnSceneGUI()
        {
            if (!asTarget.preview) return;

            Handles.color = asTarget.handlesColor;
            Vector3 viewAngleFrom = asTarget.DirectionFromAngle(-asTarget.angle / 2, false);
            Vector3 viewAngleTo = asTarget.DirectionFromAngle(asTarget.angle / 2, false);

            Handles.DrawWireArc(asTarget.transform.position, Vector3.back, viewAngleFrom, asTarget.angle, asTarget.radius);
            Handles.DrawLine(asTarget.transform.position, asTarget.transform.position + viewAngleFrom * asTarget.radius);
            Handles.DrawLine(asTarget.transform.position, asTarget.transform.position + viewAngleTo * asTarget.radius);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            #region Mesh Display
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("MESH DISPLAY", EditorStyles.boldLabel);

            meshDisplayGroupVisible.target = EditorGUILayout.Toggle("Enable Mesh Display", meshDisplayGroupVisible.target);
            drawViewMeshProperty.boolValue = meshDisplayGroupVisible.value;

            if (EditorGUILayout.BeginFadeGroup(meshDisplayGroupVisible.faded))
            {
                EditorGUILayout.PropertyField(meshResolutionProperty);
                EditorGUILayout.PropertyField(edgeDistanceThresholdProperty);
                EditorGUILayout.PropertyField(edgeResolveIterationsProperty);
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            #region Preview Options
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("PREVIEW", EditorStyles.boldLabel);

            previewGroupVisible.target = EditorGUILayout.Toggle("Enable Preview", previewGroupVisible.target);
            previewProperty.boolValue = previewGroupVisible.value;

            if (EditorGUILayout.BeginFadeGroup(previewGroupVisible.faded))
            {
                EditorGUILayout.PropertyField(handlesColorProperty);
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}

