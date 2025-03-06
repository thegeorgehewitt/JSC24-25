using UnityEngine;

namespace Custom.Controller
{
    public class CharacterControlSwitchMotor : CharacterControlBase
    {
        public override string[] InputActionKeysName
        {
            get => new string[] {
                "Switch"
            };
        }



        [Header("REFERENCES")]
        [SerializeField] private CharacterMotor2D motor;



        private void OnEnable()
        {
            GetInputActionWithName("Switch").performed += _ => Possess();
        }

        private void OnDisable()
        {
            GetInputActionWithName("Switch").performed -= _ => Possess();
        }



        private void Possess()
        {
            PlayerMotorController.Possess(motor);
        }
    }
}
