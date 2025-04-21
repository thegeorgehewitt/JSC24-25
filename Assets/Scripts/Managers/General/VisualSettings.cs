using UnityEngine;

using Custom.Scriptable.Settings;

namespace Custom.Settings
{
    public static class VisualSettings
    {
        private static VisualSettingsData instance;
        public static VisualSettingsData Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return LoadInEditor();
#endif
                if (instance == null)
                    instance = Resources.Load<VisualSettingsData>("Default Visual Settings");

                return instance;
            }
        }

        public static ColorPaletteData ColorPalette => Instance.colorPalette;



#if UNITY_EDITOR
        private static VisualSettingsData LoadInEditor()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(VisualSettingsData).Name}");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<VisualSettingsData>(path);
            }
            return null;
        }
#endif
    }
}