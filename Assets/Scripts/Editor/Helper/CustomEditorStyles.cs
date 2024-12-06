using System.Linq;

using UnityEngine;

namespace Custom.Editor.Styles
{
    public class CustomEditorStyles : ScriptableObject
    {
        [SerializeField] private GUISkin defaultSkin;

        private static CustomEditorStyles instance;
        private static CustomEditorStyles Instance
        {
            get 
            { 
                if (instance == null)
                {
                    instance = CreateInstance<CustomEditorStyles>();
                }

                return instance;
            }
        }

        public static GUIStyle foldoutHeader { get { return Instance.defaultSkin.customStyles.FirstOrDefault(m => m.name == "foldoutHeader"); } }
    }
}
