using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    public class PlayerMotorController : PlayerController
    {
        public static PlayerMotorController Instance;

        public static event Action<CharacterMotor2D> OnControlledMotorChanged;



        [Space(10)]
        [SerializeField] private CharacterMotor2D controlledMotor;

        public InputActionAsset InputAsset { get { return inputActionAsset; } }
        public CharacterMotor2D ControlledMotor { get { return controlledMotor; } }



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
        public static void PauseMotor(bool _pause)
        {
            if (!Instance.controlledMotor) return;

            Instance.controlledMotor.SetPause(_pause);

            if (_pause)
                Instance.controlledMotor.OnUnpossessed(Instance);
            else
                Instance.controlledMotor.OnPossessed(Instance);
        }
    }
}
