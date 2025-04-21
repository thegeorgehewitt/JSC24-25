using UnityEngine;

using AYellowpaper.SerializedCollections;

using Custom.Settings;
using Custom.Scriptable.Settings;

namespace Custom.Scriptable.Interactable
{
    [CreateAssetMenu(fileName = "New Interaction Data", menuName = "Custom/Interactable/Interaction Data")]
    public class ObjectInteractionData : ScriptableObject
    {
        public const int MAX_TITLE_CHARACTERS = 20;



        [Tooltip("Icon of the interaction.")]
        [Attribute.SpritePreview]
        [SerializeField] public Sprite icon;

        [Tooltip("Cost of the interaction")]
        [SerializeField] public int cost = 0;

        [Tooltip("Title of the interaction.")]
        [Attribute.LimitCharacter(MAX_TITLE_CHARACTERS)]
        [SerializeField] public string title = "New Interaction";

        [Tooltip("Rich text compatible.")]
        [Attribute.Multiline(3, 10)]
        [SerializeField] private string description;

        [Attribute.ReadOnly]
        [SerializeField] private SerializedDictionary<string, string> descriptionRichTextAlias;

        public string ParsedDescription
        {
            get
            {
                string store = description;

                foreach (var alias in descriptionRichTextAlias)
                {
                    store = store.Replace(alias.Key, alias.Value);
                }

                return store;
            }
        }



        public void OnInspectorReloaded()
        {
            descriptionRichTextAlias = new()
            {
                {"<h>", $"<color=#{ColorUtility.ToHtmlStringRGB(VisualSettings.ColorPalette.GetUIColor(UIElementGroup.Highlight))}>"},
                {"</h>", "</color>"},

                {"<kw>", $"<b><color=#{ColorUtility.ToHtmlStringRGB(VisualSettings.ColorPalette.GetUIColor(UIElementGroup.Keyword))}>"},
                {"</kw>", "</color></b>"},

                // New alias could be added here.
            };
        }
    }



    /// <summary>
    /// All listed types of enemies.
    /// </summary>
    public enum InteractableObjectType
    {
        Friendly,

        Neutral,

        Hostile,

        Chaotic,
    }
}
