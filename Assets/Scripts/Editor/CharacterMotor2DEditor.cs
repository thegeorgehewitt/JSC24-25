#if UNITY_EDITOR
using UnityEditor;

using Unity.VisualScripting;
using UnityEngine;

using Custom.Controller;

namespace Custom.Editor
{
    [CustomEditor(typeof(CharacterMotor2D), true)]
    public class CharacterMotor2DEditor : UnityEditor.Editor
    {
        private SerializedProperty actions;

        private void OnEnable()
        {
            actions = serializedObject.FindProperty("controlScripts");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Get Control Scripts in Children"))
            {
                actions.ClearArray();
                int index = 0;
                foreach (var movementScript in target.GetComponentsInChildren<CharacterControlBase>())
                {
                    actions.InsertArrayElementAtIndex(index);
                    actions.GetArrayElementAtIndex(index).objectReferenceValue = movementScript;
                    index++;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif