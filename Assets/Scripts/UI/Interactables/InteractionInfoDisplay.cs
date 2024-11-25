using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Custom.Scriptable;

namespace Custom.UI
{
    public class InteractionInfoDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Image interactionIcon;
        [SerializeField] private Image borderImage;
        [SerializeField] private TextMeshProUGUI interactionName;
        [SerializeField] private TextMeshProUGUI interactionCost;

        [Header("ACTIVE OPTION")]
        [SerializeField] private Color inactiveColor = Color.white;
        [SerializeField] private Color activeColor = Color.red;

        public void DisplayInfo(ObjectInteractionData _data, bool active)
        {
            interactionIcon.sprite = _data.icon;
            interactionName.text = _data.tag;
            interactionCost.text = _data.cost.ToString();
            borderImage.color = active? activeColor : inactiveColor;
        }
    }
}
