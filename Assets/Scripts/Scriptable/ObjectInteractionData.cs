using UnityEngine;

namespace Custom.Scriptable
{
    [CreateAssetMenu(fileName = "New Interaction Data", menuName = "Custom/Interactable/Interaction Data")]
    public class ObjectInteractionData : ScriptableObject
    {
        [Header("GENERAL")]
        [Tooltip("Icon of the interaction.")]
        public Sprite icon;
        [Tooltip("Name of the interaction.")]
        public string tag = "Interaction Name";
        [Tooltip("Cost of the interaction")]
        public int cost = 0;
    }
}
