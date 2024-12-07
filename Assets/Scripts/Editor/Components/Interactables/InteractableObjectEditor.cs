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
    public class InteractableObjectEditor : CustomBaseEditor
    {
        private SerializedProperty objectData;
        private SerializedProperty interactionData;

        private SerializedProperty spriteRenderer;

        private AnimBool expandObjectProperties;

        private bool IsExpanded
        {
            get => SessionState.GetBool($"Expanded {typeof(InteractableObjectEditor)} : {serializedObject.targetObject.GetInstanceID()}", false);
            set => SessionState.SetBool($"Expanded {typeof(InteractableObjectEditor)} : {serializedObject.targetObject.GetInstanceID()}", value);
        }



        protected virtual void OnEnable()
        {
            objectData = AssignToProperty("objectData");
            interactionData = AssignToProperty("interactionData");

            spriteRenderer = AssignToProperty("spriteRenderer");

            expandObjectProperties = new(IsExpanded);
            expandObjectProperties.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            #region Foldout
            EditorGUILayout.Space(10);

            IsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsExpanded, "Interactable Object Properties", CustomEditorStyles.foldoutHeader);
            EditorGUILayout.EndFoldoutHeaderGroup();

            expandObjectProperties.target = IsExpanded;

            if (EditorGUILayout.BeginFadeGroup(expandObjectProperties.faded))
            {
                EditorGUILayout.Space();
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
                if (!spriteRenderer.objectReferenceValue)
                {
                    EditorGUILayout.HelpBox(
                        "Missing reference for Sprite Renderer.\n" +
                        "Interaction will not be displayed properly.",
                        MessageType.Warning);
                }
                #endregion

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}
