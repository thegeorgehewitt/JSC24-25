using UnityEngine;

namespace Custom.Controller
{
    public class CharacterControlSwitchMotor : CharacterControlBase
    {
        [Header("REFERENCES")]
        [SerializeField] private PlayerController controller;
        [SerializeField] private CharacterMotor2D motor;



        private void OnEnable()
        {
            InputAction.performed += _ => Possess();
        }

        private void OnDisable()
        {
            InputAction.performed -= _ => Possess();
        }



        private void Possess()
        {
            controller.Possess(motor);
        }
    }
}
