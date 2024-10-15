using UnityEngine;

using TMPro;

using Custom.Controller;

namespace Custom.UI
{
    public class PlayerControllerDisplayTest : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private PlayerController controller;



        private void Update()
        {
            text.text = "Controlling Motor: " + controller.ControlledMotor.name;
        }
    }
}
