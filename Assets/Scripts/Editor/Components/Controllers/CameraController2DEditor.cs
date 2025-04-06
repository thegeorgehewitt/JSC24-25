using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.Controller.General;
using Custom.Utility;

namespace Custom.Editor
{
    using Styles;

    [CustomEditor(typeof(CameraController2D))]
    public class CameraController2DEditor : CustomBaseEditor
    {
        private CameraController2D asTarget;

        private SerializedProperty trackingTransform;
        private SerializedProperty enableSnapping;
        private SerializedProperty snapEaseCurve;
        private SerializedProperty snapDuration;
        private SerializedProperty speedMultiplier;
        private SerializedProperty maxSpeed;
        private SerializedProperty minSpeed;

        private SerializedProperty enableBounds;
        private SerializedProperty enableHardLock;
        private SerializedProperty outerBounds;

        private SerializedProperty minOrthographicSize;
        private SerializedProperty maxOrthographicSize;

        private AnimBool expandTracking;
        private AnimBool expandBounds;

        private bool IsTrackingExpanded
        {
            get => SessionState.GetBool($"Tracking {typeof(CameraController2DEditor)} : {serializedObject.targetObject.GetInstanceID()}", false);
            set => SessionState.SetBool($"Tracking {typeof(CameraController2DEditor)} : {serializedObject.targetObject.GetInstanceID()}", value);
        }

        private bool IsBoundsExpanded
        {
            get => SessionState.GetBool($"Bounds {typeof(CameraController2DEditor)} : {serializedObject.targetObject.GetInstanceID()}", false);
            set => SessionState.SetBool($"Bounds {typeof(CameraController2DEditor)} : {serializedObject.targetObject.GetInstanceID()}", value);
        }




        private void OnEnable()
        {
            asTarget = (CameraController2D)target;

            trackingTransform = AssignToProperty("trackingTransform");
            enableSnapping = AssignToProperty("enableSnapping");
            snapEaseCurve = AssignToProperty("snapEaseCurve");
            snapDuration = AssignToProperty("snapDuration");
            speedMultiplier = AssignToProperty("speedMultiplier");
            maxSpeed = AssignToProperty("maxSpeed");
            minSpeed = AssignToProperty("minSpeed");

            enableBounds = AssignToProperty("enableBounds");
            enableHardLock = AssignToProperty("enableHardLock");
            outerBounds = AssignToProperty("outerBounds");

            minOrthographicSize = AssignToProperty("minOrthographicSize");
            maxOrthographicSize = AssignToProperty("maxOrthographicSize");


            expandTracking = new(IsTrackingExpanded);
            expandTracking.valueChanged.AddListener(Repaint);

            expandBounds = new(IsBoundsExpanded);
            expandBounds.valueChanged.AddListener(Repaint);
        }

        private void OnSceneGUI()
        {
            if (!asTarget.enabled) return;

            // Bounds controls & displays
            if (asTarget.enableBounds)
            {
                // Bounds size controls
                Handles.color = Color.yellow;

                Vector2 sidePos;
                Vector2 sizeDiff = Vector2.zero;

                sidePos = Offset.sides[0] * asTarget.OuterBounds.extents + (Vector2)asTarget.OuterBounds.center;
                sizeDiff.x -= ((Vector2)Handles.FreeMoveHandle(sidePos, HandleUtility.GetHandleSize(sidePos) / 25.0f, Vector3.zero, Handles.DotHandleCap) - sidePos).x;

                sidePos = Offset.sides[1] * asTarget.OuterBounds.extents + (Vector2)asTarget.OuterBounds.center;
                sizeDiff.x += ((Vector2)Handles.FreeMoveHandle(sidePos, HandleUtility.GetHandleSize(sidePos) / 25.0f, Vector3.zero, Handles.DotHandleCap) - sidePos).x;

                sidePos = Offset.sides[2] * asTarget.OuterBounds.extents + (Vector2)asTarget.OuterBounds.center;
                sizeDiff.y -= ((Vector2)Handles.FreeMoveHandle(sidePos, HandleUtility.GetHandleSize(sidePos) / 25.0f, Vector3.zero, Handles.DotHandleCap) - sidePos).y;

                sidePos = Offset.sides[3] * asTarget.OuterBounds.extents + (Vector2)asTarget.OuterBounds.center;
                sizeDiff.y += ((Vector2)Handles.FreeMoveHandle(sidePos, HandleUtility.GetHandleSize(sidePos) / 25.0f, Vector3.zero, Handles.DotHandleCap) - sidePos).y;

                Undo.RecordObject(asTarget, "Change Bounds Size of CameraController2D");
                asTarget.outerBounds.size += sizeDiff;

                // Bounds position controls
                Handles.color = Color.yellow;

                Vector2 centerPos = asTarget.OuterBounds.center;
                Vector2 centerDiff = Vector2.zero;

                centerDiff = (Vector2)Handles.FreeMoveHandle(centerPos, HandleUtility.GetHandleSize(sidePos) / 25.0f, Vector3.zero, Handles.DotHandleCap) - centerPos;

                Undo.RecordObject(asTarget, "Change Bounds Position of CameraController2D");
                asTarget.outerBounds.center += centerDiff;

                // Draw bounds
                DrawBounds(asTarget.OuterBounds, "(Outer Bounds)", ValidBounds(asTarget.OuterBounds) ? Color.yellow : Color.red);
                DrawBounds(asTarget.ValidBounds, "(Valid Bounds)", ValidBounds(asTarget.ValidBounds) ? Color.green : Color.red, true);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            #region Tracking
            IsTrackingExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsTrackingExpanded, "Tracking", CustomEditorStyles.foldoutHeader);
            EditorGUILayout.EndFoldoutHeaderGroup();
            expandTracking.target = IsTrackingExpanded;

            if (EditorGUILayout.BeginFadeGroup(expandTracking.faded))
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(trackingTransform);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(enableSnapping);

                if (enableSnapping.boolValue)
                {
                    EditorGUILayout.PropertyField(snapDuration);
                    EditorGUILayout.PropertyField(snapEaseCurve);
                }
                else
                {
                    EditorGUILayout.PropertyField(speedMultiplier);
                    EditorGUILayout.PropertyField(maxSpeed);
                    EditorGUILayout.PropertyField(minSpeed);
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            #region Bounds
            IsBoundsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsBoundsExpanded, "Bounds", CustomEditorStyles.foldoutHeader);
            EditorGUILayout.EndFoldoutHeaderGroup();
            expandBounds.target = IsBoundsExpanded;

            if (EditorGUILayout.BeginFadeGroup(expandBounds.faded))
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(enableBounds);

                if (enableBounds.boolValue)
                {
                    EditorGUILayout.PropertyField(enableHardLock);
                    EditorGUILayout.PropertyField(outerBounds);
                }

                #region Zoom Constrains
                float minValue = minOrthographicSize.floatValue;
                float maxValue = maxOrthographicSize.floatValue;

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("Zoom Constrains");

                EditorGUI.indentLevel--;
                minValue = EditorGUILayout.FloatField(minValue, GUILayout.Width(40));
                EditorGUILayout.MinMaxSlider(
                    ref minValue,
                    ref maxValue,
                    0.0f,
                    GetMaxZoomConstrains());
                maxValue = EditorGUILayout.FloatField(maxValue, GUILayout.Width(40));
                EditorGUI.indentLevel++;

                EditorGUILayout.EndHorizontal();

                minOrthographicSize.floatValue = minValue;
                maxOrthographicSize.floatValue = maxValue;

                if (enableBounds.boolValue && enableHardLock.boolValue)
                {
                    EditorGUILayout.HelpBox(
                        "Max constrain is controlled by bounds.\n" +
                        "Disable bounds hard lock to use this freely.",
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



        private bool ValidBounds(Bounds _bounds)
        {
            return _bounds.size.x >= 0 && _bounds.size.y >= 0;
        }

        private void DrawBounds(Bounds _bounds, string _name, Color _color, bool _dotted = false)
        {
            Handles.color = _color;

            for (int i = 0; i < 4; i++)
            {
                Vector2 start = Offset.corners[i] * _bounds.extents + (Vector2)_bounds.center;
                Vector2 end = Offset.corners[(i + 1) % 4] * _bounds.extents + (Vector2)_bounds.center;
                if (_dotted)
                    Handles.DrawDottedLine(start, end, 4.0f);
                else
                    Handles.DrawLine(start, end);
            }

            GUIStyle labelStyle = new GUIStyle();
            labelStyle.normal.textColor = _color;
            Vector2 labelPos = GetBottomLeftCornerOffset(asTarget.OuterBounds) * _bounds.extents + (Vector2)_bounds.center;
            Handles.Label(
                labelPos + Vector2.down * HandleUtility.GetHandleSize(labelPos) * 0.1f,
                _name,
                labelStyle);
        }

        private Vector2 GetBottomLeftCornerOffset(Bounds _bounds)
        {
            if (_bounds.size.x < 0)
                if (_bounds.size.y < 0)
                    return Offset.corners[2]; // Top Right
                else
                    return Offset.corners[3]; // Bottom Right
            else
                if (_bounds.size.y < 0)
                    return Offset.corners[1]; // Top Left
                else
                    return Offset.corners[0]; // Bottom Left
        }

        private float GetMaxZoomConstrains()
        {
            if (enableBounds.boolValue && enableHardLock.boolValue)
            {
                return Mathf.Min(outerBounds.rectValue.height / 2.0f, outerBounds.rectValue.width / 2.0f / CameraController2D.MainCamera.aspect);
            }
            else
            {
                return 10.0f;
            }
        }
    }
}
