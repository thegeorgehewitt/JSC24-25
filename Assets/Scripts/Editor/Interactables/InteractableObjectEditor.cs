using System.Collections.Generic;

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
        private SerializedProperty interactionData;

        private SerializedProperty spriteRenderer;


        private List<string> excludedProperties = new List<string> { "m_Script" };
        private AnimBool expandObjectProperties;

        private bool IsExpanded
        {
            get { return SessionState.GetBool($"EXPANDED ({typeof(InteractableObjectEditor)}) : {serializedObject.targetObject.GetInstanceID()}", true); }
            set { SessionState.SetBool($"EXPANDED ({typeof(InteractableObjectEditor)}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }

        protected System.Type editorType;



        protected virtual void OnEnable()
        {
            editorType = GetType();

            objectData = serializedObject.FindProperty("objectData");
            interactionData = serializedObject.FindProperty("interactionData");

            spriteRenderer = serializedObject.FindProperty("spriteRenderer");

            expandObjectProperties = new(IsExpanded);
            expandObjectProperties.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());

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

                EditorGUILayout.PropertyField(interactionData);
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



        protected void AddExcludedProperties(params SerializedProperty[] _properties)
        {
            foreach (var property in _properties)
            {
                excludedProperties.Add(property.name);
            }
        }
    }
}
