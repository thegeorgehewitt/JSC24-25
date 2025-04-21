using UnityEngine;
using UnityEditor;

using Custom.Settings;
using Custom.Scriptable.Settings;

namespace Custom.UI.General
{
    public class VisualSettingsWindow : EditorWindow
    {
        private VisualSettingsData settings;



        [MenuItem("Tools/Custom/Visual Settings")]
        public static void ShowWindow()
        {
            GetWindow<VisualSettingsWindow>("Visual Settings");
        }



        private void OnEnable()
        {
            if (settings == null)
                settings = VisualSettings.Instance;
        }

        private void OnGUI()
        {
            if (settings == null)
            {
                EditorGUILayout.HelpBox(
                    "No GlobalSettings asset found. Create one in Assets/Resources.", 
                    MessageType.Warning);

                if (GUILayout.Button("Create Asset"))
                    CreateAsset();

                return;
            }

            SerializedObject serializedObject = new(settings);
            serializedObject.Update();

            SerializedProperty prop = serializedObject.GetIterator();
            prop.NextVisible(true);
            while (prop.NextVisible(false))
            {
                EditorGUILayout.PropertyField(prop, true);
            }

            serializedObject.ApplyModifiedProperties();
        }



        private void CreateAsset()
        {
            settings = CreateInstance<VisualSettingsData>();
            AssetDatabase.CreateAsset(settings, "Assets/Resources/Default Visual Settings.asset");
            AssetDatabase.SaveAssets();
        }
    }
}
