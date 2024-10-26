using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;

using Custom.Utility;

namespace Custom.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : UnityEditor.Editor
    {
        private FieldOfView asTarget;

        private SerializedProperty drawViewMeshProperty;
        private SerializedProperty meshResolutionProperty;
        private SerializedProperty edgeDistanceThresholdProperty;
        private SerializedProperty edgeResolveIterationsProperty;
        private SerializedProperty blockableFilterProperty;

        private SerializedProperty previewProperty;
        private SerializedProperty handlesColorProperty;

        private AnimBool drawMeshGroupVisible;
        private AnimBool previewGroupVisible;

        private void OnEnable()
        {
            asTarget = (FieldOfView)target;

            drawViewMeshProperty = serializedObject.FindProperty("drawViewMesh");
            meshResolutionProperty = serializedObject.FindProperty("meshResolution");
            edgeDistanceThresholdProperty = serializedObject.FindProperty("edgeDistanceThreshold");
            edgeResolveIterationsProperty = serializedObject.FindProperty("edgeResolveIterations");
            blockableFilterProperty = serializedObject.FindProperty("blockableFilter");

            previewProperty = serializedObject.FindProperty("preview");
            handlesColorProperty = serializedObject.FindProperty("handlesColor");

            drawMeshGroupVisible = new(asTarget.drawViewMesh);
            drawMeshGroupVisible.valueChanged.AddListener(Repaint);

            previewGroupVisible = new(asTarget.preview);
            previewGroupVisible.valueChanged.AddListener(Repaint);
        }

        private void OnSceneGUI()
        {
            if (!asTarget.preview) return;

            Handles.color = asTarget.handlesColor;
            Vector3 viewAngleFrom = asTarget.DirectionFromAngle(-asTarget.Angle / 2, false);
            Vector3 viewAngleTo = asTarget.DirectionFromAngle(asTarget.Angle / 2, false);

            Handles.DrawWireArc(asTarget.transform.position, Vector3.back, viewAngleFrom, asTarget.Angle, asTarget.Radius);
            Handles.DrawLine(asTarget.transform.position, asTarget.transform.position + viewAngleFrom * asTarget.Radius);
            Handles.DrawLine(asTarget.transform.position, asTarget.transform.position + viewAngleTo * asTarget.Radius);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            #region Mesh Display
            EditorGUILayout.PropertyField(drawViewMeshProperty);
            drawMeshGroupVisible.target = drawViewMeshProperty.boolValue;

            if (EditorGUILayout.BeginFadeGroup(drawMeshGroupVisible.faded))
            {
                EditorGUILayout.PropertyField(meshResolutionProperty);
                EditorGUILayout.PropertyField(edgeDistanceThresholdProperty);
                EditorGUILayout.PropertyField(edgeResolveIterationsProperty);
                EditorGUILayout.PropertyField(blockableFilterProperty);
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            #region Preview Options
            EditorGUILayout.PropertyField(previewProperty);
            previewGroupVisible.target = previewProperty.boolValue;

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

