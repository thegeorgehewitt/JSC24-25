using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;

        public static event Action<CharacterMotor2D> OnControlledMotorChanged;

        [Header("REFERENCE")]
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private CharacterMotor2D controlledMotor;

        public InputActionAsset InputAsset { get { return inputActionAsset; } }
        public CharacterMotor2D ControlledMotor { get { return controlledMotor; } }



#if UNITY_EDITOR
        private void Reset()
        {
            // Get default InputActionAsset.
            // Remove this incase of performance lost when adding PlayerController component.
            var inputAssets = Resources.FindObjectsOfTypeAll<InputActionAsset>();
            if (inputAssets.Length > 0) inputActionAsset = inputAssets[0];
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
            inputActionAsset.Disable();

            // Possess default motor.
            if (controlledMotor) Possess(controlledMotor);
        }



        /// <summary>
        /// Enable an <see cref="InputActionMap"/> in the referenced <see cref="inputActionAsset">inputAction</see>.
        /// </summary>
        /// <param name="_map"> The <see cref="InputActionMap"/> to enable. </param>
        public void EnableActionMap(InputActionMap _map)
        {
            if (_map == null) return;
            if (_map.enabled) return;

            inputActionAsset.FindActionMap(_map.id).Enable();
        }

        /// <summary>
        /// Disable an <see cref="InputActionMap"/> in the referenced <see cref="inputActionAsset">inputAction</see>.
        /// </summary>
        /// <param name="_map"> The <see cref="InputActionMap"/> to disable. </param>
        public void DisableActionMap(InputActionMap _map)
        {
            if (_map == null) return;
            if (!_map.enabled) return;

            inputActionAsset.FindActionMap(_map.id).Disable();
        }

        /// <summary>
        /// Enable an <see cref="InputActionMap"/> in the referenced <see cref="inputActionAsset">inputAction</see>.
        /// </summary>
        /// <param name="_action"> The <see cref="InputAction"/> to enable. </param>
        public void EnableAction(InputAction _action)
        {
            if (_action == null) return;
            if (_action.enabled) return;

            inputActionAsset.FindAction(_action.id).Enable();
        }

        /// <summary>
        /// Disable an <see cref="InputAction"/> in the referenced <see cref="inputActionAsset">inputAction</see>.
        /// </summary>
        /// <param name="_action"> The <see cref="InputAction"/> to disable. </param>
        public void DisableAction(InputAction _action)
        {
            if (_action == null) return;
            if (!_action.enabled) return;

            inputActionAsset.FindAction(_action.id).Disable();
        }



        /// <summary>
        /// Set the controlling motor to a new <see cref="CharacterMotor2D"/>.
        /// </summary>
        /// <param name="_motor"> The <see cref="CharacterMotor2D"/> to possess. </param>
        public static void Possess(CharacterMotor2D _motor)
        {
            Instance.controlledMotor?.OnUnpossessed(Instance);
            Instance.controlledMotor = _motor;
            Instance.controlledMotor?.OnPossessed(Instance);

            OnControlledMotorChanged?.Invoke(Instance.controlledMotor);
        }

        /// <summary>
        /// If this controller was controlling the given <see cref="CharacterMotor2D"/>, stop controlling it. <br/>
        /// Otherwise, this function do nothing.
        /// </summary>
        /// <param name="_motor"> The <see cref="CharacterMotor2D"/> to unpossess. </param>
        public static void Unpossess(CharacterMotor2D _motor)
        {
            if (Instance.controlledMotor != _motor) return;

            Instance.controlledMotor?.OnUnpossessed(Instance);
            Instance.controlledMotor = null;

            OnControlledMotorChanged?.Invoke(Instance.controlledMotor);
        }



        /// <summary>
        /// Freeze the <see cref="controlledMotor"/> any input registered to PlayerController.
        /// </summary>
        public static void PauseMotor()
        {
            if (!Instance.controlledMotor) return;

            Instance.controlledMotor.SetPause(true);
            Instance.controlledMotor.OnUnpossessed(Instance);
        }
    }
}
