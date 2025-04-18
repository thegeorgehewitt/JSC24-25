using UnityEngine;
using UnityEditor;

using Custom.Scriptable;
using Unity.VisualScripting;

namespace Custom.Editor
{
    [CustomEditor(typeof(ObjectiveData))]
    public class ObjectiveDataEditor : CustomBaseEditor
    {
        private SerializedProperty label;
        private SerializedProperty description;
        private SerializedProperty objectiveType;
        private SerializedProperty trackMode;

        private Vector2 scrollPosition;


        private void OnEnable()
        {
            label = AssignToProperty("label");
            description = AssignToProperty("description");
            objectiveType = AssignToProperty("objectiveType");
            trackMode = AssignToProperty("trackMode");

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }



        public override void OnInspectorGUI()
        {
            // Setup styles
            GUIStyle wrappedTextStyle = new(EditorStyles.textArea)
            {
                wordWrap = true
            };

            GUIStyle leftAlignedTextStyle = new(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleRight
            };

            serializedObject.Update();

            EditorGUILayout.PropertyField(objectiveType);
            EditorGUILayout.PropertyField(trackMode);
            EditorGUILayout.Space();

            #region Label
            EditorGUILayout.BeginVertical();

            EditorGUILayout.PropertyField(label);
            EditorGUILayout.LabelField($"({label.stringValue.Length}/{ObjectiveData.MAX_TITLE_LENGTH})", leftAlignedTextStyle);

            EditorGUILayout.EndVertical();

            if (label.stringValue.Length > ObjectiveData.MAX_TITLE_LENGTH)
            {
                label.stringValue = label.stringValue[..ObjectiveData.MAX_TITLE_LENGTH];
                
            }
            if (label.stringValue.Length == ObjectiveData.MAX_TITLE_LENGTH)
            {
                EditorGUILayout.HelpBox(
                    $"Objective title can not exceed {ObjectiveData.MAX_TITLE_LENGTH} letters.",
                    MessageType.Warning);
            }
            #endregion

            #region Description
            float minHeight = wrappedTextStyle.fontSize + 4f;
            float maxHeight = minHeight * 10f;
            float calculatedHeight = wrappedTextStyle.CalcHeight(new GUIContent(description.stringValue), EditorGUIUtility.currentViewWidth - 30);
            float clampedHeight = Mathf.Clamp(calculatedHeight, minHeight, maxHeight);

            EditorGUILayout.BeginVertical();

            EditorGUILayout.PrefixLabel("Description");

            EditorGUI.BeginChangeCheck();
            if (calculatedHeight > maxHeight)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(clampedHeight));
                description.stringValue = EditorGUILayout.TextArea(description.stringValue, wrappedTextStyle, GUILayout.ExpandHeight(true));
            }
            else
            {
                description.stringValue = EditorGUILayout.TextArea(description.stringValue, wrappedTextStyle, GUILayout.Height(clampedHeight));
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Description Updated.");
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            if (calculatedHeight > maxHeight)
                EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
            #endregion

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }



        private void OnUndoRedoPerformed()
        {
            Repaint();

            EditorUtility.SetDirty(target);
        }
    }
}
