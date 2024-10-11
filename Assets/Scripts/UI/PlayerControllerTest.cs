using UnityEngine;

using TMPro;

using Custom.Controller;

namespace Custom.UI
{
    public class PlayerControllerTest : MonoBehaviour
    {
        [Header("REFREENCES")]
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private PlayerController controller;



        private void Update()
        {
            text.text = "Controlling Motor: " + controller.ControlledMotor.name;
        }
    }
}
