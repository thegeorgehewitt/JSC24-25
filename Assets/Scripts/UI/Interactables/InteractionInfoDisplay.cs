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
        [SerializeField] private GameObject costObject;

        [Header("ACTIVE OPTION")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.cyan;
        [SerializeField] private Color disabledColor = Color.grey;



        private Color GetColorFromState(InteractionState _state)
        {
            switch(_state)
            {
                case InteractionState.Normal:
                    return normalColor;

                case InteractionState.Selected:
                    return selectedColor;

                case InteractionState.Disabled:
                    return disabledColor;

                default: return normalColor;
            }
        }



        public void DisplayInfo(ObjectInteractionData _data, InteractionState _state)
        {
            borderImage.color = GetColorFromState(_state);

            if (!_data) return;

            interactionIcon.sprite = _data.icon;
            interactionName.text = _data.tag;

            costObject.SetActive(_data.cost != 0);
            interactionCost.text = _data.cost.ToString();
        }
    }



    public enum InteractionState
    {
        Normal,
        Selected,
        Disabled
    }
}
