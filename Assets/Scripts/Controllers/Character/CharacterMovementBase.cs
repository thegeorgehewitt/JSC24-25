using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    public abstract class CharacterMovementBase : MonoBehaviour
    {
        [SerializeField] protected InputActionReference inputAction;
        [HideInInspector] public CharacterMotor attachedMotor;

        public InputAction InputAction { get { return inputAction.action; } }
        public InputActionMap InputActionMap { get { return inputAction.action.actionMap; } }
    }
}