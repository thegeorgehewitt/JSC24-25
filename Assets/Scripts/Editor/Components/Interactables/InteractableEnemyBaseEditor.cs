using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.Interactable.Character.Enemy;

namespace Custom.Editor
{
    using Styles;

    [CustomEditor(typeof(InteractableEnemyBase), true)]
    public class InteractableEnemyBaseEditor : InteractableCharacterBaseEditor
    {
        private SerializedProperty minDetectLevel;

        private AnimBool expandProperties;

        private bool IsExpanded
        {
            get => SessionState.GetBool($"Expanded {typeof(InteractableEnemyBaseEditor)} : {serializedObject.targetObject.GetInstanceID()}", false);
            set => SessionState.SetBool($"Expanded {typeof(InteractableEnemyBaseEditor)} : {serializedObject.targetObject.GetInstanceID()}", value);
        }



        protected override void OnEnable()
        {
            base.OnEnable();

            minDetectLevel = AssignToProperty("minDetectLevel");

            expandProperties = new(IsExpanded);
            expandProperties.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            #region Foldout
            IsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsExpanded, "Interactable Enemy Properties", CustomEditorStyles.foldoutHeader);
            EditorGUILayout.EndFoldoutHeaderGroup();

            expandProperties.target = IsExpanded;

            if (EditorGUILayout.BeginFadeGroup(expandProperties.faded))
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(minDetectLevel);

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}
