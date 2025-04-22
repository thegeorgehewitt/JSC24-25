using System;

using UnityEngine;

namespace Custom.Scriptable.Settings
{
    [Serializable]
    public class VisualSettingsData : ScriptableObject
    {
        [SerializeField] public ColorPaletteData colorPalette;
        [SerializeField] public OutlineSettingsData outlineSettings;
    }
}
