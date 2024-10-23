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

        private AnimBool inputActionVisible;
        private SerializedProperty passiveControlProperty;
        private SerializedProperty inputActionProperty;



        private void OnEnable()
        {
            asTarget = (CharacterControlBase)target;

            passiveControlProperty = serializedObject.FindProperty("passiveControl");
            inputActionProperty = serializedObject.FindProperty("inputAction");

            inputActionVisible = new(asTarget.IsPassiveControl);
            inputActionVisible.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            #region Controls
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("CONTROLS", EditorStyles.boldLabel);

            if (!asTarget.inputAction && !asTarget.IsPassiveControl)
            {
                EditorGUILayout.HelpBox(
                    "Missing InputAction reference for active control.\n" +
                    "Please add InputActionReference or change control to passive.", MessageType.Error);
            }

            inputActionVisible.value = EditorGUILayout.Toggle("Is Passive Control", inputActionVisible.value);
            passiveControlProperty.boolValue = inputActionVisible.value;

            if (EditorGUILayout.BeginFadeGroup(1 - inputActionVisible.faded))
            {
                EditorGUILayout.PropertyField(inputActionProperty);
            }
            EditorGUILayout.EndFadeGroup();
            #endregion
            
            serializedObject.ApplyModifiedProperties();

            DrawPropertiesExcluding(serializedObject, "m_Script");
        }
    }
}
#endif