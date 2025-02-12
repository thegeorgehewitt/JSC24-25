using System.IO;

using UnityEditor;
using UnityEngine;

namespace Custom.FSM.Editor
{
    public class StateMachineFileHandler : AssetPostprocessor
    {
        [UnityEditor.Callbacks.OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            string assetPath = AssetDatabase.GetAssetPath(instanceID);

            if (assetPath.EndsWith(".fsm"))
            {
                StateMachineEditorWindow.OpenEditorWindow(assetPath);
                return true;
            }

            return false;
        }



        #region File IO
        /// <summary>
        /// Saves the state machine to an <c>.fsm</c> file in JSON format.
        /// </summary>
        /// <param name="_object">   The state machine object to save. </param>
        /// <param name="_filePath"> The file path where the object will be saved. </param>
        public static void SaveToFile(StateMachine _object, string _filePath)
        {
            string json = JsonUtility.ToJson(_object, true);

            File.WriteAllText(_filePath, json);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Loads a state machine from an <c>.fsm</c> file.
        /// </summary>
        /// <param name="_filePath"> The file path from which the object will be loaded. </param>
        /// <returns>
        /// The loaded state machine object if the file exists; otherwise <see langword="null"/>.
        /// </returns>
        public static StateMachine LoadFromFile(string _filePath)
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);

                return JsonUtility.FromJson<StateMachine>(json);
            }

            return null;
        }
        #endregion
    }
}
