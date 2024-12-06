using UnityEditor;
using UnityEditor.AnimatedValues;

using Custom.Controller;

namespace Custom.Editor
{
    using Styles;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(CharacterControlBase), true)]
    public class CharacterControlBaseEditor : CustomBaseEditor
    {
        private CharacterControlBase asTarget;

        private SerializedProperty passiveControl;
        private SerializedProperty inputActionsDictionary;
        private SerializedProperty inputActionsDictKeys;
        private SerializedProperty inputActionsDictValues;

        private AnimBool expandControls;
        private bool invalidActions;

        private bool IsExpanded
        {
            get { return SessionState.GetBool($"EXPANDED ({this.GetType()}) : {serializedObject.targetObject.GetInstanceID()}", false); }
            set { SessionState.SetBool($"EXPANDED ({this.GetType()}) : {serializedObject.targetObject.GetInstanceID()}", value); }
        }



        private void OnEnable()
        {
            asTarget = (CharacterControlBase)target;

            passiveControl = AssignToProperty("passiveControl");
            inputActionsDictionary = AssignToProperty("inputActions");
            inputActionsDictKeys = inputActionsDictionary.FindPropertyRelative("keys");
            inputActionsDictValues = inputActionsDictionary.FindPropertyRelative("values");

            expandControls = new(IsExpanded);
            expandControls.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);

            #region Controls
            IsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(IsExpanded, "Controls", CustomEditorStyles.foldoutHeader);
            expandControls.target = IsExpanded;
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (EditorGUILayout.BeginFadeGroup(expandControls.faded))
            {
                EditorGUILayout.PropertyField(passiveControl);

                if (!passiveControl.boolValue)
                {
                    foreach (var key in asTarget.InputActionKeysName)
                    {
                        if (ContainsKeyInSerializedDictionary(key)) continue;
                        
                        AddKeyToSerializedDictionary(key);
                    }

                    DrawInputActionList();
                }

                if (invalidActions && !passiveControl.boolValue)
                {
                    EditorGUILayout.HelpBox(
                        "Missing InputActionReference for Active Control.\n" +
                        "Please add a reference to each inputs or enable Passive Control.",
                        MessageType.Error);

                    asTarget.SetActive(false);
                }

                EditorGUILayout.Space(10);
            }

            EditorGUILayout.EndFadeGroup();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }



        private bool ContainsKeyInSerializedDictionary(string key)
        {
            for (int i = 0; i < inputActionsDictKeys.arraySize; i++)
            {
                if (inputActionsDictKeys.GetArrayElementAtIndex(i).stringValue == key)
                    return true;
            }
            return false;
        }

        private void AddKeyToSerializedDictionary(string key)
        {
            // Add the key to the keys list
            inputActionsDictKeys.InsertArrayElementAtIndex(inputActionsDictKeys.arraySize);
            inputActionsDictKeys.GetArrayElementAtIndex(inputActionsDictKeys.arraySize - 1).stringValue = key;

            // Add a default value to the values list
            inputActionsDictValues.InsertArrayElementAtIndex(inputActionsDictValues.arraySize);
            SerializedProperty newValueProperty = inputActionsDictValues.GetArrayElementAtIndex(inputActionsDictValues.arraySize - 1);
            newValueProperty.objectReferenceValue = null; // Default to null
        }

        private void DrawInputActionList()
        {
            // If not input actions found, notify user and cancel drawing input action list.
            if (asTarget.InputActionKeysName.Length == 0)
            {
                EditorGUILayout.HelpBox(
                    "This control script does not contains any input actions.\n" +
                    "To add input actions, override InputActionKeysName within script.",
                    MessageType.Info);

                return;
            }

            // Styles
            float elementHeight = 18.0f;
            float actionsWidth = 120.0f;
            Vector4 padding = new Vector4(5, 5, 0, 10);

            // Backgrounds
            EditorGUILayout.Space(5);

            Rect actionColRect = EditorGUILayout.GetControlRect(false, 0);
            actionColRect.position -= new Vector2(padding.x, padding.z);
            actionColRect.width = actionsWidth;
            actionColRect.height = elementHeight * (asTarget.InputActionKeysName.Length + 1) + padding.z + padding.w;
            Rect inputReference = new();
            inputReference.position = actionColRect.position + new Vector2(actionsWidth, 0);
            inputReference.width = Screen.width - actionColRect.width;
            inputReference.height = actionColRect.height;

            EditorGUI.DrawRect(actionColRect, new Color(0.185f, 0.185f, 0.185f));
            EditorGUI.DrawRect(inputReference, new Color(0.2f, 0.2f, 0.2f));

            // Header
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Action Name", EditorStyles.boldLabel, GUILayout.Width(actionsWidth), GUILayout.Height(elementHeight));
            EditorGUILayout.LabelField("Input Action Reference", EditorStyles.boldLabel);

            EditorGUILayout.EndHorizontal();

            // List
            invalidActions = false;

            for (int i = 0; i < asTarget.InputActionKeysName.Length; i++)
            {
                string key = asTarget.InputActionKeysName[i];

                EditorGUILayout.BeginHorizontal();

                // Prefix label from string list
                EditorGUILayout.LabelField($"{key}", GUILayout.Width(actionsWidth), GUILayout.Height(elementHeight));

                // Find corresponding value in the dictionary
                SerializedProperty valueReference = inputActionsDictValues.GetArrayElementAtIndex(i);

                // Display the value using PropertyField
                EditorGUILayout.PropertyField(valueReference, new GUIContent(""), GUILayout.Height(elementHeight));

                EditorGUILayout.EndHorizontal();

                if (valueReference.objectReferenceValue == null)
                {
                    invalidActions = true;
                }
            }
        }
    }
}