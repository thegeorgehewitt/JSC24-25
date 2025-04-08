using UnityEngine;
using UnityEditor;

namespace Custom.Editor.Tools
{
    public class SceneInitializer : EditorWindow
    {
        private const string TARGET_FOLDER = "Assets/Scenes/";
        private const string BASE_SCENE_PATH = TARGET_FOLDER + "Base Scene Structure (DO NOT EDIT).unity";

        private string newSceneName;
        private string validationMessage;
        private bool isNameValid;



        [MenuItem("Tools/Custom/Scene Initializer")]
        public static void CreateNewBaseScene()
        {
            GetWindow<SceneInitializer>(false, "Scene Initializer", true);
        }



        private void OnGUI()
        {
            EditorGUILayout.Space();

            newSceneName = EditorGUILayout.TextField(new GUIContent("Scene Name"), newSceneName);

            // Scene name validation.
            ValidateSceneName();

            if (!string.IsNullOrEmpty(validationMessage))
            {
                EditorGUILayout.HelpBox(
                    validationMessage, 
                    isNameValid ? MessageType.Info : MessageType.Error);
            }

            // Create new scene button.
            EditorGUI.BeginDisabledGroup(!isNameValid);
            if (GUILayout.Button("Create Scene"))
            {
                CreateNewScene();
            }
            EditorGUI.EndDisabledGroup();
        }



        private void ValidateSceneName()
        {
            if (string.IsNullOrWhiteSpace(newSceneName))
            {
                validationMessage = "Scene name cannot be empty.";
                isNameValid = false;
                return;
            }

            if (newSceneName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
            {
                validationMessage = "Scene name contains invalid characters.";
                isNameValid = false;
                return;
            }

            string targetPath = $"{TARGET_FOLDER}{newSceneName}.unity";
            if (System.IO.File.Exists(targetPath))
            {
                validationMessage = "A scene with this name already exists.";
                isNameValid = false;
                return;
            }

            validationMessage = "Scene name is valid.";
            isNameValid = true;
        }

        private void CreateNewScene()
        {
            // Check for existing base scene structure.
            if (!System.IO.File.Exists(BASE_SCENE_PATH))
            {
                Debug.LogError($"Base scene structure not found at path: {BASE_SCENE_PATH}");
                return;
            }

            string targetPath = $"{TARGET_FOLDER}{newSceneName}.unity";

            // Copy the base scene to the new location
            if (AssetDatabase.CopyAsset(BASE_SCENE_PATH, targetPath))
            {
                Debug.Log($"Scene created successfully at: {targetPath}");
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Scene creation failed.");
            }
        }
    }
}
