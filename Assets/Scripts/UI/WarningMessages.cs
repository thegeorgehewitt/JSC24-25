using System.Collections;

using UnityEngine;

using TMPro;

using Custom.Controller;

namespace Custom.UI
{
    public class WarningMessages : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI text;

        [Header("MESSAGE")]
        [SerializeField] private float messageShowDuration;
        [SerializeField] private Color defaultWarningColor = new Color(0.8f, 0.1f, 0.1f);

        private Coroutine popupCoroutine;



        private void OnEnable()
        {
            CharacterControlInteract.OnInteractObjectOutOfRange += OnInteractObjectOutOfRange;
            CharacterControlInteract.OnVisionBlocked += OnVisionBlocked;
        }

        private void OnDisable()
        {
            CharacterControlInteract.OnInteractObjectOutOfRange -= OnInteractObjectOutOfRange;
            CharacterControlInteract.OnVisionBlocked -= OnVisionBlocked;
        }

        private void Awake()
        {
            text.enabled = false;
        }



        private void OnInteractObjectOutOfRange()
        {
            UpdateText(
                "< OBJECT OUT OF RANGE >",
                defaultWarningColor
            );
        }

        private void OnVisionBlocked()
        {
            UpdateText(
                "< VIEW IS OBSTRUCTED >",
                defaultWarningColor
            );
        }

        private void UpdateText(string _text, Color _color)
        {
            text.text = _text;
            text.color = _color;

            if (popupCoroutine != null) StopCoroutine(popupCoroutine);
            popupCoroutine = StartCoroutine(PopupCoroutine());
        }

        private IEnumerator PopupCoroutine()
        {
            text.enabled = true;
            yield return new WaitForSeconds(messageShowDuration);
            text.enabled = false;
        }
    }
}
