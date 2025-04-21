using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Custom.Editor
{
    public class CustomBaseEditor : UnityEditor.Editor
    {
        private List<string> excludedProperties = new () { "m_Script" };



        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());

            if (GUILayout.Button(
                $"This inspector is controlled by a custom editor.\n" +
                $"Click this to locate the custom editor file used for this script.", 
                EditorStyles.helpBox))
            {
                HighlightAsset(GetScriptFilePath());
            }

            serializedObject.ApplyModifiedProperties();
        }



        protected SerializedProperty AssignToProperty(string _propertyName)
        {
            excludedProperties.Add(_propertyName);

            return serializedObject.FindProperty(_propertyName);
        }



        private void HighlightAsset(string assetPath)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            if (asset == null) return;

            EditorGUIUtility.PingObject(asset);
        }

        public string GetScriptFilePath()
        {
            MonoScript script = MonoScript.FromScriptableObject(this);
            if (script == null)
            {
                Debug.LogError("Could not find script file.");
                return null;
            }

            string path = AssetDatabase.GetAssetPath(script);
            return path;
        }
    }
}
