using UnityEngine;
using UnityEditor;

using Custom.Scriptable;

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
            serializedObject.Update();

            EditorGUILayout.PropertyField(objectiveType);
            EditorGUILayout.PropertyField(trackMode);

            EditorGUILayout.Space();

            CustomGUIDrawers.LimitedTextField(label, ObjectiveData.MAX_TITLE_LENGTH);
            CustomGUIDrawers.MultiLineTextField(description, ref scrollPosition);

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
