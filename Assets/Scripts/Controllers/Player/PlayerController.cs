using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;

        public static Action<CharacterMotor2D> OnControlledMotorChanged;

        [Header("REFERENCE")]
        [SerializeField] private InputActionAsset inputAction;
        [SerializeField] private CharacterMotor2D controlledMotor;

        public InputActionAsset InputAsset { get { return inputAction; } }
        public CharacterMotor2D ControlledMotor { get { return controlledMotor; } }



#if UNITY_EDITOR
        private void Reset()
        {
            // Get default InputActionAsset.
            // Remove this incase of performance lost when adding PlayerController component.
            var inputAssets = Resources.FindObjectsOfTypeAll<InputActionAsset>();
            if (inputAssets.Length > 0) inputAction = inputAssets[0];
        }
#endif

        private void Awake()
        {
            #region Singleton
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            #endregion
        }

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
            if (_map.enabled) return;

            inputAction.FindActionMap(_map.id).Enable();
        }

        /// <summary>
        /// Disable an <see cref="InputActionMap"/> in the referenced <see cref="inputAction"/>.
        /// </summary>
        /// <param name="_map"> The <see cref="InputActionMap"/> to disable. </param>
        public void DisableActionMap(InputActionMap _map)
        {
            if (_map == null) return;
            if (!_map.enabled) return;

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



        /// <summary>
        /// Pause any input registered to PlayerController.
        /// </summary>
        public static void PauseMotor()
        {
            if (!Instance.controlledMotor) return;

            Instance.controlledMotor.paused = true;
            Instance.controlledMotor.OnUnpossessed(Instance);
        }
    }
}
