using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace Custom.UI
{
    public class InteractableObjectStateDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private ContentSizeFitter contentSizeFitter;



#if UNITY_EDITOR
        private void Reset()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            contentSizeFitter = GetComponentInChildren<ContentSizeFitter>();
        }
#endif



        public void DisplayInfo(string _text)
        {
            DisplayInfo(_text, text.color);
        }

        public void DisplayInfo(string _text, Color _color)
        {
            text.text = _text;
            text.color = _color;
        }

        public void SetActive(bool _state)
        {
            // For some reason we need to reset the ContentSizeFitter for the elements to resize properly.
            // Might need fixing in the future.
            contentSizeFitter.enabled = false;
            contentSizeFitter.enabled = true;

            gameObject.SetActive(_state);
        }
    }
}
