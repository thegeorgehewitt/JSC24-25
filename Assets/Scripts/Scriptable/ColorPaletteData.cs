using UnityEngine;

using AYellowpaper.SerializedCollections;

namespace Custom.Scriptable.Settings
{
    [CreateAssetMenu(fileName = "New Color Palette", menuName = "Custom/Color Palette")]
    public class ColorPaletteData : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<UIElementGroup, Color> UIPalette;



        /// <summary>
        /// Get the color of the <see cref="UIElementGroup"/> in the current palette.
        /// </summary>
        /// <param name="_group">   The <see cref="UIElementGroup"/> to retrieve color from. </param>
        /// <returns>
        /// If palette contains the given <see cref="UIElementGroup"/>, return the assigned color in that palette. <br/>
        /// Otherwise, return <see cref="Color.clear"/>.
        /// </returns>
        public Color GetUIColor(UIElementGroup _group)
        {
            if (!UIPalette.ContainsKey(_group)) return Color.clear;

            return UIPalette[_group];
        }
    }



    /// <summary>
    /// All color groups used in UI.
    /// </summary>
    public enum UIElementGroup
    {
        FriendlyPrimary = 1,

        NeutralPrimary = 2,

        HostilePrimary = 3,

        ChaoticPrimary = 4,



        Highlight = 10,

        Keyword = 11,
    }
}
