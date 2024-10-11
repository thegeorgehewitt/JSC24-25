using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    public class PlayerController : MonoBehaviour
    {
        [Header("REFERENCE")]
        [SerializeField] private InputActionAsset inputAction;
        [SerializeField] private CharacterMotor2D controlledMotor;

        public InputActionAsset InputAsset { get { return inputAction; } }
        public CharacterMotor2D ControlledMotor { get { return controlledMotor; } }

        public static Action<CharacterMotor2D> OnControlledMotorChanged;



#if UNITY_EDITOR
        private void Reset()
        {
            // Get default InputActionAsset.
            // Remove this incase of performance lost when adding PlayerController component.
            var inputAssets = Resources.FindObjectsOfTypeAll<InputActionAsset>();
            if (inputAssets.Length > 0) inputAction = inputAssets[0];
        }
#endif

        private void Start()
        {
            // Disable all inputs.
            inputAction.Disable();

            // Possess default motor.
            if (controlledMotor) Possess(controlledMotor);
        }



        /// <summary>
        /// Enable an <see cref="InputActionMap"/> in the referenced <see cref="inputAction"/>.
        /// </summary>
        /// <param name="_map"> The <see cref="InputActionMap"/> to enable. </param>
        public void EnableActionMap(InputActionMap _map)
        {
            if (_map == null) return;

            inputAction.FindActionMap(_map.id).Enable();
        }

        /// <summary>
        /// Disable an <see cref="InputActionMap"/> in the referenced <see cref="inputAction"/>.
        /// </summary>
        /// <param name="_map"> The <see cref="InputActionMap"/> to disable. </param>
        public void DisableActionMap(InputActionMap _map)
        {
            if (_map == null) return;

            inputAction.FindActionMap(_map.id).Disable();
        }

        /// <summary>
        /// Set the controlling motor to a new <see cref="CharacterMotor2D"/>.
        /// </summary>
        /// <param name="_motor"> The <see cref="CharacterMotor2D"/> to possess. </param>
        public void Possess(CharacterMotor2D _motor)
        {
            controlledMotor.OnUnpossessed(this);
            controlledMotor = _motor;
            controlledMotor.OnPossessed(this);

            OnControlledMotorChanged?.Invoke(controlledMotor);
        }
    }
}
