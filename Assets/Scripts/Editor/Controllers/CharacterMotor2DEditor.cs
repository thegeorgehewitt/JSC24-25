#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using Custom.Controller;

namespace Custom.Editor
{
    [CustomEditor(typeof(CharacterMotor2D), true)]
    public class CharacterMotor2DEditor : UnityEditor.Editor
    {
        private CharacterMotor2D asTarget;

        private SerializedProperty actions;



        private void OnEnable()
        {
            asTarget = (CharacterMotor2D)target;

            actions = serializedObject.FindProperty("controlScripts");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Get Control Scripts in Children"))
            {
                actions.ClearArray();
                int index = 0;
                foreach (var movementScript in asTarget.GetComponentsInChildren<CharacterControlBase>())
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