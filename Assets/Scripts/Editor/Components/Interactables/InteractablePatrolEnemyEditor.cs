using UnityEngine;
using UnityEditor;

using Custom.Interactable.Character.Enemy;

namespace Custom.Editor
{
    [CustomEditor(typeof(InteractablePatrolEnemy))]
    public class InteractablePatrolEnemyEditor : InteractableEnemyBaseEditor
    {
        private SerializedProperty patrolPoints;



        protected override void OnEnable()
        {
            base.OnEnable();

            patrolPoints = AssignToProperty("patrolPoints");
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();

            // Draw patrol path.
            int previousIndex = -1;
            for (int i = 0; i < patrolPoints.arraySize; i++)
            {
                patrolPoints.GetArrayElementAtIndex(i).vector3Value = 
                    CreatePatrolPoint(
                        patrolPoints.GetArrayElementAtIndex(i).vector3Value,
                        GetColorFromPalette(i * 0.1f));

                if (previousIndex != -1)
                {
                    DrawArrowBetweenPoints(
                        patrolPoints.GetArrayElementAtIndex(previousIndex).vector3Value,
                        patrolPoints.GetArrayElementAtIndex(i).vector3Value,
                        GetColorFromPalette(previousIndex * 0.1f),
                        GetColorFromPalette(i * 0.1f),
                        i == patrolPoints.arraySize - 1 && patrolPoints.arraySize == 2);
                }

                previousIndex = i;
            }

            if (patrolPoints.arraySize > 0)
            {
                GUIStyle middleText = new GUIStyle(EditorStyles.label);
                middleText.alignment = TextAnchor.LowerCenter;

                Handles.Label(
                    patrolPoints.GetArrayElementAtIndex(0).vector3Value + Vector3.up * HandleUtility.GetHandleSize(Vector3.zero) * 0.1f,
                    "(Start)", middleText);
            }

            // Draw a complete loop.
            DrawArrowBetweenPoints(
                patrolPoints.GetArrayElementAtIndex(patrolPoints.arraySize - 1).vector3Value,
                patrolPoints.GetArrayElementAtIndex(0).vector3Value,
                GetColorFromPalette((patrolPoints.arraySize - 1) * 0.1f),
                GetColorFromPalette(0));

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(patrolPoints);

            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }



        #region Draw Utils
        private Vector3 CreatePatrolPoint(Vector3 _position, Color? _color = null)
        {
            Color originalColor = Handles.color;
            Handles.color = _color ?? originalColor;

            Vector3 newPos = Handles.FreeMoveHandle(_position, HandleUtility.GetHandleSize(_position) * 0.1f, EditorSnapSettings.move, Handles.CubeHandleCap);

            Handles.color = originalColor;

            return newPos;
        }

        private void DrawArrowBetweenPoints(Vector3 _start, Vector3 _end, Color? _startColor = null, Color? _endColor = null, bool _bothDirection = false)
        {
            Vector3 direction = (_end - _start).normalized;
            float size = HandleUtility.GetHandleSize(_end) * 0.1f;
            _end -= direction * size;

            if (_bothDirection) _start += direction * size;

            CustomSceneViewUtil.DrawGradientLine(_start, _end, _startColor, _endColor);

            if (_bothDirection)
            {
                CustomSceneViewUtil.DrawSolidArrowhead(_start, -direction, size * 1.5f, _startColor);
                CustomSceneViewUtil.DrawSolidArrowhead(_end, direction, size * 1.5f, _endColor);
            }
            else
            {
                CustomSceneViewUtil.DrawSolidArrowhead(_end, direction, size * 1.5f, _endColor);
            }
        }

        public Color GetColorFromPalette(float _normalizedValue, float _saturation = 0.8f, float _brightness = 0.8f)
        {
            _normalizedValue = _normalizedValue % 1;
            return Color.HSVToRGB(_normalizedValue, _saturation, _brightness);
        }
        #endregion
    }
}
