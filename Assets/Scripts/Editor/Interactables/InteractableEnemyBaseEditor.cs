using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.Interactable.Enemy;

namespace Custom.Editor
{
    using Styles;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractableEnemyBase), true)]
    public class InteractableEnemyBaseEditor : InteractableObjectEditor
    {
        private InteractableEnemyBase asTarget;

        private SerializedProperty fieldOfView;
        private SerializedProperty flip;
        private SerializedProperty maxRange;
        private SerializedProperty angle;
        private SerializedProperty trackableLayers;
        private SerializedProperty blockableLayers;

        private AnimBool expandEnemyProperties;
        private AnimBool showTargetDetectionProperties;

        private bool IsExpanded
        {
            get { return SessionState.GetBool($"EXPANDED ({typeof(InteractableEnemyBaseEditor)}) : {serializedObject.targetObject.GetInstanceID()}", true); }
            set { SessionState.SetBool($"EXPANDED ({typeof(InteractableEnemyBaseEditor)}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }



        protected override void OnEnable()
        {
            base.OnEnable();

            asTarget = (InteractableEnemyBase)target;

            editorType = typeof(InteractableEnemyBaseEditor);

            fieldOfView = serializedObject.FindProperty("fieldOfView");
            flip = serializedObject.FindProperty("flip");
            maxRange = serializedObject.FindProperty("maxRange");
            angle = serializedObject.FindProperty("angle");
            trackableLayers = serializedObject.FindProperty("trackableLayers");
            blockableLayers = serializedObject.FindProperty("blockableLayers");

            expandEnemyProperties = new(IsExpanded);
            expandEnemyProperties.valueChanged.AddListener(Repaint);

            showTargetDetectionProperties = new(fieldOfView.objectReferenceValue);
            showTargetDetectionProperties.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            IsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsExpanded, "Interactable Enemy Properties", CustomEditorStyles.foldoutHeader);
            expandEnemyProperties.target = IsExpanded;

            if (EditorGUILayout.BeginFadeGroup(expandEnemyProperties.faded))
            {
                EditorGUI.indentLevel++;

                #region Target Detection
                EditorGUILayout.PropertyField(fieldOfView);
                if (!fieldOfView.objectReferenceValue)
                {
                    EditorGUILayout.HelpBox(
                        "Missing reference for Field Of View.",
                        MessageType.Error);
                }
                else
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(flip);
                    EditorGUILayout.PropertyField(maxRange);
                    EditorGUILayout.PropertyField(angle);

                    EditorGUILayout.PropertyField(trackableLayers);
                    if (trackableLayers.intValue == 0)
                    {
                        EditorGUILayout.HelpBox(
                            "Tracking Layers is not set." +
                            "Enemy will not be able to detect objects.",
                            MessageType.Warning);
                    }

                    EditorGUILayout.PropertyField(blockableLayers);
                }
                #endregion

                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
