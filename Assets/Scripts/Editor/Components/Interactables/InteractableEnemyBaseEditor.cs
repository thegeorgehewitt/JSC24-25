using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.Interactable.Character.Enemy;

namespace Custom.Editor
{
    [CustomEditor(typeof(InteractableEnemyBase), true)]
    public class InteractableEnemyBaseEditor : InteractableCharacterBaseEditor
    {
        private SerializedProperty lockOnDuration;
        private SerializedProperty normalizedLockOnDistance;

        private SerializedProperty minVisibilityDetectLevel;
        private SerializedProperty baseDetectRate; 
        private SerializedProperty baseIgnoreRate;

        private AnimBool expandProperties;

        private bool IsExpanded
        {
            get => SessionState.GetBool($"Expanded {typeof(InteractableEnemyBaseEditor)} : {serializedObject.targetObject.GetInstanceID()}", false);
            set => SessionState.SetBool($"Expanded {typeof(InteractableEnemyBaseEditor)} : {serializedObject.targetObject.GetInstanceID()}", value);
        }



        protected override void OnEnable()
        {
            base.OnEnable();

            lockOnDuration = AssignToProperty("lockOnDuration");
            normalizedLockOnDistance = AssignToProperty("normalizedLockOnDistance");

            minVisibilityDetectLevel = AssignToProperty("minVisibilityDetectLevel");
            baseDetectRate = AssignToProperty("baseDetectRate");
            baseIgnoreRate = AssignToProperty("baseIgnoreRate");

            expandProperties = new(IsExpanded);
            expandProperties.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            #region Foldout
            IsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsExpanded, "Interactable Enemy Properties", CustomGUIStyles.foldoutHeader);
            EditorGUILayout.EndFoldoutHeaderGroup();

            expandProperties.target = IsExpanded;

            if (EditorGUILayout.BeginFadeGroup(expandProperties.faded))
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(minVisibilityDetectLevel);
                EditorGUILayout.PropertyField(baseDetectRate);
                EditorGUILayout.PropertyField(baseIgnoreRate);

                EditorGUILayout.PropertyField(lockOnDuration);
                EditorGUILayout.PropertyField(normalizedLockOnDistance);

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}
