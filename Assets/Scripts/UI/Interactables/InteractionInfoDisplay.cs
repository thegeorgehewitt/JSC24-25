using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Custom.Settings;
using Custom.Scriptable.Interactable;
using Custom.Scriptable.Settings;

namespace Custom.UI.HUD
{
    public class InteractionInfoDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Image interactionIcon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Image costIcon;
        [Space]
        [SerializeField] private CanvasGroup mainCanvasGroup;
        [SerializeField] private Image highLightImage;
        [SerializeField] private Image overlayImage;



        private void Awake()
        {
            Color temp = VisualSettings.ColorPalette.GetUIColor(UIElementGroup.HostilePrimary);
            temp.a = 0.2f;
            overlayImage.color = temp;
        }



        public void DisplayInfo(ObjectInteractionData _data, InteractionState _state)
        {
            // Interaction state display
            overlayImage.enabled = _state == InteractionState.Disabled;
            mainCanvasGroup.alpha = _state == InteractionState.Selected ? 1.0f : 0.4f;
            highLightImage.pixelsPerUnitMultiplier = _state == InteractionState.Selected ? 1 : 2;

            if (!_data) return;

            // Basic info display
            interactionIcon.sprite = _data.icon;
            nameText.text = _data.title;

            // Cost display
            costText.text = Mathf.Abs(_data.cost).ToString();

            UIElementGroup costGroup = UIElementGroup.NeutralPrimary;
            if (_data.cost > 0)
                costGroup = UIElementGroup.HostilePrimary;
            else if (_data.cost < 0)
                costGroup = UIElementGroup.FriendlyPrimary;

            costText.color = VisualSettings.ColorPalette.GetUIColor(costGroup);
            costIcon.color = VisualSettings.ColorPalette.GetUIColor(costGroup);
        }
    }



    public enum InteractionState
    {
        Normal,

        Selected,

        Disabled
    }
}
