using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.Interactable;

namespace Custom.Editor
{
    using Styles;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractableObject), true)]
    public class InteractableObjectEditor : UnityEditor.Editor
    {
        [SerializeField] private Texture2D foldoutBackgroundOff;
        [SerializeField] private Texture2D foldoutBackgroundOn;

        private SerializedProperty objectData;
        private SerializedProperty interactionDatas;

        private SerializedProperty spriteRenderer;

        private AnimBool expandObjectProperties;

        protected System.Type editorType;

        private bool IsExpanded
        {
            get { return SessionState.GetBool($"EXPANDED ({typeof(InteractableObjectEditor)}) : {serializedObject.targetObject.GetInstanceID()}", true); }
            set { SessionState.SetBool($"EXPANDED ({typeof(InteractableObjectEditor)}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }



        protected virtual void OnEnable()
        {
            editorType = typeof(InteractableObjectEditor);

            objectData = serializedObject.FindProperty("objectData");
            interactionDatas = serializedObject.FindProperty("interactionData");

            spriteRenderer = serializedObject.FindProperty("spriteRenderer");

            expandObjectProperties = new(IsExpanded);
            expandObjectProperties.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                $"This inspector is controlled by a custom editor.\n" +
                $"Edit this in {editorType} script.",
                MessageType.None);

            EditorGUILayout.Space(10);

            DrawPropertiesExcluding(serializedObject, "m_Script");

            #region Foldout
            EditorGUILayout.Space(10);

            IsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsExpanded, "Interactable Object Properties", CustomEditorStyles.foldoutHeader);
            expandObjectProperties.target = IsExpanded;
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (EditorGUILayout.BeginFadeGroup(expandObjectProperties.faded))
            {
                EditorGUI.indentLevel++;

                #region Data
                EditorGUILayout.PropertyField(objectData);

                if (!objectData.objectReferenceValue)
                {
                    EditorGUILayout.HelpBox(
                        "Missing reference for interactable object data.",
                        MessageType.Warning);
                }

                EditorGUILayout.PropertyField(interactionDatas);
                #endregion

                #region Interaction Area
                EditorGUILayout.PropertyField(spriteRenderer);

                if (!spriteRenderer.objectReferenceValue)
                {
                    EditorGUILayout.HelpBox(
                        "Missing reference for Sprite Renderer.\n" +
                        "Interaction will not be displayed properly.",
                        MessageType.Warning);
                }
                #endregion

                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}
