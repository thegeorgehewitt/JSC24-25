using System.Linq;

using UnityEngine;

namespace Custom.Editor
{
    public class CustomGUIStyles : ScriptableObject
    {
        [SerializeField] private GUISkin defaultSkin;

        private static CustomGUIStyles instance;
        private static CustomGUIStyles Instance
        {
            get 
            { 
                if (instance == null)
                {
                    instance = CreateInstance<CustomGUIStyles>();
                }

                return instance;
            }
        }

        public static GUIStyle foldoutHeader => Instance.defaultSkin.customStyles.FirstOrDefault(m => m.name == "foldoutHeader");
        public static GUIStyle toolbarButton => Instance.defaultSkin.customStyles.FirstOrDefault(m => m.name == "toolbarButton");
    }
}
