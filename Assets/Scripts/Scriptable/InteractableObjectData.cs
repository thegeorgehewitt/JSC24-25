using UnityEngine;

using AYellowpaper.SerializedCollections;

using Custom.Settings;
using Custom.Scriptable.Settings;

namespace Custom.Scriptable.Interactable
{
    [CreateAssetMenu(fileName = "New Interactable Object Data", menuName = "Custom/Interactable/Interactable Object Data")]
    public class InteractableObjectData : ScriptableObject
    {
        public const int MAX_TITLE_CHARACTERS = 20;



        [Tooltip("Display icon in interactable popup.")]
        [Attribute.SpritePreview]
        [SerializeField] public Sprite icon;

        [Tooltip("Type of the object.")]
        [SerializeField] public InteractableObjectType type;

        [Tooltip("Name of the object.")]
        [Attribute.LimitCharacter(MAX_TITLE_CHARACTERS)]
        [SerializeField] public string objectName = "Object Name";

        [Tooltip("")]
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
                {"<highlight>", $"<b><color=#{ColorUtility.ToHtmlStringRGB(VisualSettings.ColorPalette.GetUIColor(UIElementGroup.Highlight))}>"},
                {"</highlight>", "</color></b>"},

                // Add new alias here.
            };
        }
    }
}

