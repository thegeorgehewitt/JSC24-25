#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.Controller;

namespace Custom.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CharacterControlBase), true)]
    public class CharacterControlBaseEditor : UnityEditor.Editor
    {
        private CharacterControlBase asTarget;

        private SerializedProperty passiveControlProperty;
        private SerializedProperty inputActionProperty;
        private SerializedProperty controlGroupFoldoutProperty;

        private AnimBool inputActionVisible;
        private AnimBool foldout;



        private void OnEnable()
        {
            asTarget = (CharacterControlBase)target;

            passiveControlProperty = serializedObject.FindProperty("passiveControl");
            inputActionProperty = serializedObject.FindProperty("inputAction");
            controlGroupFoldoutProperty = serializedObject.FindProperty("controlGroupFoldout");

            inputActionVisible = new(!passiveControlProperty.boolValue);
            inputActionVisible.valueChanged.AddListener(Repaint);

            foldout = new(controlGroupFoldoutProperty.boolValue);
            foldout.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            #region Controls
            foldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(foldout.value, "CONTROLS");
            controlGroupFoldoutProperty.boolValue = foldout.value;

            if (EditorGUILayout.BeginFadeGroup(foldout.faded))
            {
                if (!inputActionProperty.objectReferenceValue && !passiveControlProperty.boolValue)
                {
                    EditorGUILayout.HelpBox(
                        "Missing InputAction reference for Active Control.\n" +
                        "Please add a reference to an InputAction or enable Passive Control.", 
                        MessageType.Error);
                }

                EditorGUILayout.PropertyField(passiveControlProperty);
                inputActionVisible.value = !passiveControlProperty.boolValue;

                if (EditorGUILayout.BeginFadeGroup(inputActionVisible.faded))
                {
                    EditorGUILayout.PropertyField(inputActionProperty);
                }
                EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            DrawPropertiesExcluding(serializedObject, "m_Script");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif