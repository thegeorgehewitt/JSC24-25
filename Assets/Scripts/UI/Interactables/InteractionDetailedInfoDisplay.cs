using UnityEngine;

using TMPro;

using Custom.Scriptable.Interactable;

namespace Custom.UI.HUD
{
    public class InteractionDetailedInfoDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI interactionName;
        [SerializeField] private TextMeshProUGUI description;



        public void UpdateDisplay(ObjectInteractionData _data)
        {
            if (_data == null) return;

            interactionName.text = _data.title;
            description.text = _data.ParsedDescription;
        }
    }
}
