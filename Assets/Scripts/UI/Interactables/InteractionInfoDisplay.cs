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
        [SerializeField] private TextMeshProUGUI interactionName;
        [SerializeField] private TextMeshProUGUI interactionCost;



        public void DisplayInfo(ObjectInteractionData _data)
        {
            interactionIcon.sprite = _data.icon;
            interactionName.text = _data.tag;
            interactionCost.text = _data.cost.ToString();
        }
    }
}
