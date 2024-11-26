using UnityEditor;

using Custom.Controller;

namespace Custom.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CharacterControlBase), true)]
    public class CharacterControlBaseEditor : UnityEditor.Editor
    {
        private CharacterControlBase asTarget;

        private SerializedProperty passiveControl;
        private SerializedProperty inputAction;



        private void OnEnable()
        {
            asTarget = (CharacterControlBase)target;

            passiveControl = serializedObject.FindProperty("passiveControl");
            inputAction = serializedObject.FindProperty("inputAction");
        }

        public override void OnInspectorGUI()
        {
            #region Controls
            EditorGUILayout.PropertyField(passiveControl);
            if (!passiveControl.boolValue)
            {
                EditorGUILayout.PropertyField(inputAction);
            }

            if (!inputAction.objectReferenceValue && !passiveControl.boolValue)
            {
                EditorGUILayout.HelpBox(
                    "Missing InputAction reference for Active Control.\n" +
                    "Please add a reference to an InputAction or enable Passive Control.",
                    MessageType.Error);
            }
            #endregion

            DrawPropertiesExcluding(serializedObject, 
                "m_Script",
                "passiveControl",
                "inputAction");

            serializedObject.ApplyModifiedProperties();
        }
    }
}